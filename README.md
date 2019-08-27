# container-performance


This repository contains few small application that I have used to compare performance of IPC/Remoting in .NET framework (various technologies i.e. TCP channels, SignalR and gRPC) running inside **windows containers** (both ```windows server container``` and ```hyperV``` container).

## TCP Channel - WCF 

How to run:
 
- Go to the project **TcpNetFramework**
- Build docker image:

Run the server with following command:
```
docker run -it -p 8789:8789 moimhossain/wcf-tcp-net-framework:latest
```

To run client
```
docker run -it moimhossain/wcf-tcp-net-framework:latest
```

> Please use ```--isolation=process|hyperv``` switch to change container isolation modes

## SignalR

How to run:
 
- Go to the project **SignalR**
- Build docker image:

Run the server with following command:
```
docker run -it -p 8790:8790 moimhossain/signalr-net-framework:latest
```

To run client
```
docker run -it moimhossain/signalr-net-framework:latest
```

> Please use ```--isolation=process|hyperv``` switch to change container isolation modes

## gRPC

How to run:
 
- Go to the project **gRPC**
- Build docker image:

Run the server with following command:
```
docker run -it -p 7888:7888 moimhossain/grpc-net-framework:latest
```

To run client
```
docker run -it moimhossain/grpc-net-framework:latest
```

> Please use ```--isolation=process|hyperv``` switch to change container isolation modes




### IPC via Shared memory (Queue) on UNIX

The demo that uses shared memory on UNIX

#### How to build

Build using the following docker command:

```
docker build -t moimhossain/unix-shared-memory-demo .
```

#### How to run Producer/Writer

Use the followig command to run the producer

```
docker run --ipc shareable --name producer moimhossain/unix-shared-memory-demo -producer
```

```
 docker rm -v producer
```

> Notice: You need to specify the IPC namespace as **shareable** so Docker engine will map the namespace to the consumer/reader container.

#### How to run the Consumer/Reader

Use the following command to run the consumer:

```
 docker run --name consumer --ipc container:producer moimhossain/unix-shared-memory-demo -consumer
```

And for clean up:
```
 docker rm -v consumer
```

# That's it!
