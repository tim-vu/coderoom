FROM nginx:alpine

COPY wait-for wait-for
RUN chmod +x ./wait-for

COPY nginx.conf /etc/nginx/nginx.conf
