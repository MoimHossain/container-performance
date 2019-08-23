

# Container: moimhossain/mmf-net-framework:latest
# Run 
#     Server: docker run -it --ipc="host" moimhossain/mmf-net-framework:latest
#     Client: docker run -it moimhossain/mmf-net-framework:latest

#FROM mcr.microsoft.com/dotnet/framework/runtime:4.7.2-windowsservercore-ltsc2019
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8

WORKDIR /app


COPY ./bin/release/ ./
ENTRYPOINT ["C:\\app\\MmfIpc.exe"]
