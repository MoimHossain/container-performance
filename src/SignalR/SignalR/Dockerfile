

# Container: moimhossain/signalr-net-framework:latest
# Run 
#     Server: docker run -it -p 8790:8790 moimhossain/signalr-net-framework:latest
#     Client: docker run -it moimhossain/signalr-net-framework:latest

#FROM mcr.microsoft.com/dotnet/framework/runtime:4.7.2-windowsservercore-ltsc2019
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8

WORKDIR /app
EXPOSE 8790

COPY ./bin/release/ ./
ENTRYPOINT ["C:\\app\\SignalR.exe"]
