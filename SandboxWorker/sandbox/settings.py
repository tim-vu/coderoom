import os

CURRENT_DIR = os.path.abspath(os.path.dirname(__file__))

TIME_LIMIT = 5
MEMORY_LIMIT = '100m'

DOCKER_IMAGE_NAME = 'coderoom-sandbox'
DOCKER_IMAGE_TAG = 'v4'
DOCKER_IMAGE_FULL_NAME = f'{DOCKER_IMAGE_NAME}:{DOCKER_IMAGE_TAG}'
DOCKERFILE_DIR = os.path.join(CURRENT_DIR, 'docker')

DOCKER_RUNTIME = 'runsc'

CODE_DIR = os.path.join(CURRENT_DIR, 'code')

