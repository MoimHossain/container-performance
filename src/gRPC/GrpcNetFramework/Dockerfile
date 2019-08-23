

# Container: moimhossain/grpc-net-framework:latest
# Run 
#     Server: docker run -it -p 7888:7888 moimhossain/grpc-net-framework:latest
#     Client: docker run -it moimhossain/grpc-net-framework:latest

#FROM mcr.microsoft.com/dotnet/framework/runtime:4.7.2-windowsservercore-ltsc2019
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8

WORKDIR /app
EXPOSE 7888

COPY ./bin/release/ ./
ENTRYPOINT ["C:\\app\\GrpcNetFramework.exe"]
