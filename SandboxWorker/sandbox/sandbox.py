import logging
import os
import shutil
import sys
import time
import uuid
from threading import Thread
from typing import List

import docker
from docker.errors import BuildError, ImageNotFound, APIError
from docker.models.containers import Container
from docker.types import Mount

from sandbox import settings
from sandbox.Language import Languages

logger = logging.getLogger('worker-sandbox')
logging.basicConfig(stream=sys.stdout, level=logging.DEBUG)


class SandboxError(Exception):

    def __init__(self, message):
        self.message = message
        super().__init__(message)


class MemoryLimitExceededError(SandboxError):

    def __init__(self, message):
        super().__init__(message)


class UnsupportedLanguageError(SandboxError):

    def __init__(self, message):
        super().__init__(message)


class TimeLimitExceededError(SandboxError):

    def __init__(self, message):
        super().__init__(message)


class NoMainFileError(SandboxError):

    def __init__(self, message):
        super().__init__(message)


class Sandbox:

    TIME_LIMIT_STEP = 0.1
    OUTPUT_FILE_NAME = 'output.txt'

    def __init__(self, code_volume, time_limit=settings.TIME_LIMIT, memory_limit=settings.MEMORY_LIMIT):
        self.code_volume = code_volume
        self.client = docker.from_env()
        self.worker_id = str(uuid.uuid4())
        self.time_limit = time_limit + Sandbox.TIME_LIMIT_STEP
        self.memory_limit = memory_limit

        self.container = None
        self.event_stream = None
        self.docker_event_listener = None

        self.memory_limit_exceeded = False
        self.time_limit_exceeded = False

        try:
            self.image = self.client.images.get(settings.DOCKER_IMAGE_FULL_NAME)
        except ImageNotFound:
            logger.info('Image %s not found. Building from dockerfile' % settings.DOCKER_IMAGE_FULL_NAME)
            self.image = self.build_image()

        if not os.path.isdir(settings.CODE_DIR):
            os.mkdir(settings.CODE_DIR)

    def __del__(self):
        self.clean_up()

    @property
    def task_directory(self):
        return os.path.join(settings.CODE_DIR, self.worker_id)

    @property
    def output_file_path(self):
        return os.path.join(self.task_directory, Sandbox.OUTPUT_FILE_NAME)

    @property
    def is_running(self):
        return bool(self.client.containers.list(filters={
            'id': self.container.id
        }))

    def build_image(self) -> tuple:

        try:
            return self.client.images.build(path=settings.DOCKERFILE_DIR, tag=settings.DOCKER_IMAGE_FULL_NAME)
        except BuildError:
            logger.exception('Unable to build image')

    def create_container(self, run_command: List[str], work_dir) -> Container:
        code_mount = Mount(source=self.code_volume, target='/home')

        try:
            return self.client.containers.create(image=settings.DOCKER_IMAGE_FULL_NAME,
                                                 runtime=settings.DOCKER_RUNTIME,
                                                 working_dir=work_dir,
                                                 command=run_command,
                                                 mounts=[code_mount],
                                                 mem_limit=self.memory_limit,
                                                 network_disabled=True,
                                                 network_mode=None,
                                                 privileged=False,
                                                 detach=True)

        except (ImageNotFound, APIError):
            logger.exception("Unable to create container")
            raise

    def kill_container(self):

        if not self.is_running:
            return

        try:
            self.container.kill()
        except APIError:
            logger.exception("Unable to kill the container")
            raise

    def remove_container(self):

        try:
            if len(self.client.containers.list(filters={'id': self.container.id}, all=True)) == 0:
                return
        except APIError:
            logger.exception("Unable to remove the container")
            raise

        if self.is_running:
            self.kill_container()

        try:
            self.container.remove()
        except APIError:
            logger.exception("Unable to remove the container")
            raise

    def run(self, language_key, code_files):

        language = Languages.get_language_by_key(language_key)

        if language is None:
            raise UnsupportedLanguageError(f'The given language {language_key} is not supported')

        if language.main_file not in map(lambda e: e.filename, code_files):
            raise NoMainFileError(f'Main file {language.main_file} not found')

        os.mkdir(self.task_directory)

        logger.debug(f'Making task directory {self.task_directory}')

        for file in code_files:
            path = os.path.join(self.task_directory, file.filename)

            logger.debug(f'Writing {file.filename} to {path}')

            with open(path, 'wb') as f:
                f.write(file.content)

        command = ['/bin/bash', '-c', f'{language.run_command} &> {Sandbox.OUTPUT_FILE_NAME}']

        logger.debug(f'Executing command "{command}" inside container')

        self.container = self.create_container(command, f'/home/{self.worker_id}')
        start_time = time.time()

        # noinspection PyBroadException
        try:
            self.start_container()

            execution_time = round(time.time() - start_time, 2)

            with open(self.output_file_path, 'r') as f:
                output = f.read()

            output = language.process_output(output)

            logger.debug(f'Command finished execution, output: {output}')
            logger.debug(f'Output after processing: {output}')
    
            return RunResult(output, execution_time)

        except BaseException:

            if self.container is not None:
                logger.debug(self.container.logs())

            logger.exception('Exception occurred during execution')
            raise
        finally:
            self.clean_up()

    def start_container(self):

        self.docker_event_listener = Thread(target=self.listen_for_docker_events)
        self.docker_event_listener.start()

        self.container.start()

        timespan = 0
        while self.is_running and timespan < self.time_limit:
            time.sleep(Sandbox.TIME_LIMIT_STEP)
            timespan += Sandbox.TIME_LIMIT_STEP

        try:

            if self.is_running:
                self.kill_container()
                self.time_limit_exceeded = True

        except APIError:
            logger.exception("Unable to kill the container")
            raise
        finally:
            self.event_stream.close()
            self.docker_event_listener.join()

        if self.memory_limit_exceeded:
            raise MemoryLimitExceededError("The program exceeded the maximum amount of memory")

        if self.time_limit_exceeded:
            raise TimeLimitExceededError("The program took too long to complete")

    def listen_for_docker_events(self):

        self.event_stream = self.client.events(decode=True, filters={
            'container': self.container.id
        })

        for event in self.event_stream:

            if event['status'] == 'oom':
                self.memory_limit_exceeded = True

    def clean_up(self):
        self.remove_container()

        if os.path.isdir(self.task_directory):
            shutil.rmtree(self.task_directory, ignore_errors=True)


class RunResult:

    def __init__(self, output: str, execution_time: float):
        self.output = output
        self.execution_time = execution_time

    def __repr__(self) -> str:
        return f'Execution time: {self.execution_time}, output: {self.output}'
