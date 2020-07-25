import base64
import os
import uuid
from typing import List

import pika

import settings
from settings import RABBITMQ_HOST, RABBITMQ_TASK
import code_execution_pb2 as jobs

CODE_DIR = "/home/timvu/Documents/Projects/dotNET/CodeRoom/Task"

task = jobs.ExecutionJob()
task.job_id = str(uuid.uuid4())
task.room_id = str(uuid.uuid4())
task.language = 2

for file in os.listdir(CODE_DIR):
    path = os.path.join(CODE_DIR, file)

    if not os.path.isfile(path):
        continue

    with open(path, 'br') as file_stream:
        f = task.files.add()
        f.filename = file
        f.content = file_stream.read()

credentials = pika.PlainCredentials('user123', 'pass123')

connection = pika.BlockingConnection(pika.ConnectionParameters("localhost", credentials=credentials))
channel = connection.channel()

channel.queue_declare(queue=RABBITMQ_TASK)

channel.basic_publish(exchange='', routing_key=RABBITMQ_TASK, body=task.SerializeToString())

connection.close()
