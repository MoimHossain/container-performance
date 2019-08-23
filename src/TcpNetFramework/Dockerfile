

# Container: moimhossain/wcf-tcp-net-framework:latest
# Run 
#     Server: docker run -it -p 8789:8789 moimhossain/wcf-tcp-net-framework:latest
#     Client: docker run -it moimhossain/wcf-tcp-net-framework:latest

#FROM mcr.microsoft.com/dotnet/framework/runtime:4.7.2-windowsservercore-ltsc2019
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8

WORKDIR /app
EXPOSE 8789

COPY ./bin/release/ ./
ENTRYPOINT ["C:\\app\\TcpNetFramework.exe"]
