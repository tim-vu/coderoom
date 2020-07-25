FROM python:3.9.0b3-alpine3.12

RUN apk add --no-cache netcat-openbsd

WORKDIR /app

COPY ./requirements.txt ./

RUN pip install -r requirements.txt

COPY . .

RUN chmod +x ./wait-for

CMD ["python", "worker.py"]
