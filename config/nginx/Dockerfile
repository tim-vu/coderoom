FROM nginx:alpine

COPY wait-for wait-for
RUN chmod +x ./wait-for

COPY nginx.conf /etc/nginx/nginx.conf
COPY localhost.crt /etc/ssl/certs/localhost.crt
COPY localhost.key /etc/ssl/private/localhost.key
