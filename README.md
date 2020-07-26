# CodeRoom

CodeRoom is an online platform with the aim to provide a streamlined experience for upcoming students to practice one-on-one interviews.

![](https://i.imgur.com/nU0brr2.png)

## Supported features

- Create a room and invite your peer
- Realtime collaborative code editor
- Group call
- Multiple programming languages supported
- Code execution

## CodeRoom architecture

CodeRoom consists of 3 major parts. The frontend, the backend and the sandboxworker. For a more in depth overview on each part check out the readme in their respective directory.

### Frontend

The frontend is a SPA written in TypeScript using React. Rooms are created through a simple REST API. Realtime communication with the backend is done through signalR. State-management is done with Redux.

### SandboxWorker

The sandboxworker is responsible for executing payloads. This is done by creating a docker container with limited permissions in which the payload is ran. The worker is written in python.

### Backend

The backend is written in C# using ASP.NET core. The design of the system follows Clean architecture. Rooms and related data are stored in a redis server. Communication between the backend and the sandbox workers is done through a (simple) servicebus. The service bus is managed by RabbitMQ

## Running (and debugging) CodeRoom

In order to run CodeRoom the following software must be installed on the host machine (only the first 4 are required to run (not debug) CodeRoom):

- .NET Core Runtime 3.1
- Node.js
- Docker
- gVisor (application kernel for the sandbox)
- Python 3.8

At this time the backend is not yet containerized.

### Starting CodeRoom

The following commands starts CodeRoom, these commands are for linux based systems, windows may differ.

```
# In the root directory
# Set the following environment variables (REQUIRED, values can be different)
export RABBITMQ_PASS='pass123'
export RABBITMQ_USER='user123'

# Start the containers
docker-compose up

# Navigate to the WebUI project
cd Backend/Src/WebUI

# Restore packages
dotnet restore

# Set environment variable
export ASPNETCORE_ENVIRONMENT=Development

# Start the backend
dotnet run
```

CodeRoom is now available at `http://localhost:5000`
