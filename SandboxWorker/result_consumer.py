import pika
from google.protobuf.message import DecodeError

import settings
from settings import RABBITMQ_TASK_COMPLETED, RABBITMQ_HOST
import code_execution_pb2 as jobs

credentials = pika.PlainCredentials('user123', 'pass123')

connection = pika.BlockingConnection(pika.ConnectionParameters("localhost", credentials=credentials))
channel = connection.channel()

channel.queue_declare(queue='ResultConsumer')
channel.queue_bind(exchange=settings.BROKER_NAME, queue='ResultConsumer', routing_key=settings.RABBITMQ_TASK_COMPLETED)


def receive_task(ch, method, properties, body):
    try:
        job_result = jobs.ExecutionJobResult()
        job_result.ParseFromString(body)

        print(job_result.output)
    except DecodeError:
        pass


channel.basic_consume(queue='ResultConsumer', on_message_callback=receive_task, auto_ack=True)
channel.start_consuming()
