import os

RABBITMQ_HOST = "rabbitmq"
RABBITMQ_TASK = "ExecutionJob"
RABBITMQ_TASK_COMPLETED = "ExecutionJobResult"

#RABBITMQ_HOST = "localhost"
#RABBITMQ_USERNAME = "user123"
#RABBITMQ_PASSWORD = "pass123"
#EXECUTABLES_VOLUME = "coderoom_executables"

RABBITMQ_USERNAME = os.environ.get('RABBITMQ_USERNAME')
RABBITMQ_PASSWORD = os.environ.get('RABBITMQ_PASSWORD')

BROKER_NAME = "coderoom_event_bus"

RUNTIME = 'dockerd' if os.environ.get('ENVIRONMENT') == 'test' else 'runmc'
EXECUTABLES_VOLUME = os.environ.get('EXECUTABLES_VOLUME')

JOB_TIMEOUT = int(os.environ.get('JOB_TIMEOUT'))
JOB_DURATION = 1000
