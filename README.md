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