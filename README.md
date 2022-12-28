# Distributed Data Bus

## Architecture
<p align="center">
  <img src="https://github.com/gs1993/DistributedDataBus/blob/main/docs/Architecture.jpg">
</p>

## Features

### gRPC
Used for synchronous communication between API Gateway and microservices, due to its lightweight nature

### Asynchronous messaging - RabbitMq
1. Easy scalable - multiple consumer instances can be added as needed to handle request pick
1. Resiliency - if one service fails on an asynchronous system,  other services will not be affected. The task will be held until the service is up and running again.

### API Gateway
Stateless API Gateway to the system act as single entry point for the clients
<p align="center">
  <img src="https://github.com/gs1993/DistributedDataBus/blob/main/docs/ApiGateway.jpg">
</p>

Benefits:
1. Horizontal scalability
2. Cache
3. Rate limitting - protection against Denial Of Service (DOS) attacks and degraded performance due to traffic spikes

--------------

## Setup

1. Download and install [Docker](https://docs.docker.com/get-docker/)
2. Go to /docker
3. Run elasticsearch
```cmd
elasticsearch-kibana-docker-compose.yaml
```
4. Run RabbitMq
```cmd
docker run -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
```
5. Start Gataway and Services
5. Run project and go to app url: [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)
