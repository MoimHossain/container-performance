

# Container: moimhossain/named-pipe-net-framework:latest
# Run 
#     Server: docker run -it -v \\.\pipe\helloworld:\\.\pipe\helloworld  moimhossain/named-pipe-net-framework:latest
#     Client: docker run -it -v \\.\pipe\helloworld:\\.\pipe\helloworld  moimhossain/named-pipe-net-framework:latest

#FROM mcr.microsoft.com/dotnet/framework/runtime:4.7.2-windowsservercore-ltsc2019
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8

WORKDIR /app
EXPOSE 8789

COPY ./pipelist64.exe ./
COPY ./pipelist.exe ./
COPY ./bin/release/ ./
ENTRYPOINT ["C:\\app\\Namedpipe.exe"]
