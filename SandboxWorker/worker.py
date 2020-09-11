import logging

import pika

import settings
import code_execution_pb2 as jobs
from sandbox import settings as sandbox_settings
from sandbox.sandbox import Sandbox, MemoryLimitExceededError, TimeLimitExceededError, NoMainFileError, \
    UnsupportedLanguageError

from settings import *

logging.getLogger('pika').setLevel(logging.WARNING)
logging.getLogger('urllib3').setLevel(logging.WARNING)
logger = logging.getLogger('worker')
logger.setLevel(logging.DEBUG)

credentials = pika.PlainCredentials(settings.RABBITMQ_USERNAME, settings.RABBITMQ_PASSWORD)

logger.info('RabbitMQ client is trying to connect')
connection = pika.BlockingConnection(pika.ConnectionParameters(RABBITMQ_HOST, credentials=credentials))
channel = connection.channel()

logger.info(f'RabbitMQ client acquired a connection to \'{settings.RABBITMQ_HOST}\'')

channel.exchange_declare(exchange=settings.BROKER_NAME, arguments= {
    'x-message-ttl': settings.JOB_TIMEOUT - settings.JOB_DURATION
})

channel.queue_declare(queue=RABBITMQ_TASK)
channel.queue_bind(exchange=settings.BROKER_NAME, queue=RABBITMQ_TASK, routing_key=settings.RABBITMQ_TASK)


def send_result(result):
    result_msg = result.SerializeToString()
    channel.basic_publish(exchange=settings.BROKER_NAME, routing_key=RABBITMQ_TASK_COMPLETED, body=result_msg)


def receive_task(ch, method, properties, body):

    logger.debug('Execution task received')

    job = jobs.ExecutionJob()
    job.ParseFromString(body)

    if len(job.files) == 0:
        ch.basic_ack(delivery_tag=method.delivery_tag)
        return

    task_result = jobs.ExecutionJobResult()
    task_result.job_id = job.job_id
    task_result.room_id = job.room_id

    task_result.error = True
    task_result.execution_time = 0

    # noinspection PyBroadException
    try:

        sandbox = Sandbox(settings.EXECUTABLES_VOLUME)
        result = sandbox.run(job.language, job.files)

        task_result.error = False
        task_result.error_message = ''
        task_result.output = result.output
        task_result.execution_time = result.execution_time
    except TimeLimitExceededError as e:
        task_result.error_message = e.message
        task_result.execution_time = sandbox_settings.TIME_LIMIT
    except (UnsupportedLanguageError, NoMainFileError, MemoryLimitExceededError, ) as e:
        task_result.error_message = e.message
        task_result.execution_time = 0
    except Exception:
        logger.exception("Unable to execute code")
        task_result.error_message = 'Something went wrong'
        task_result.execution_time = 0

    ch.basic_ack(delivery_tag=method.delivery_tag)
    send_result(task_result)


channel.basic_qos(prefetch_count=1)
channel.basic_consume(queue=RABBITMQ_TASK, on_message_callback=receive_task, auto_ack=False)

logger.info("Worker is waiting to consume execution tasks")
channel.start_consuming()
