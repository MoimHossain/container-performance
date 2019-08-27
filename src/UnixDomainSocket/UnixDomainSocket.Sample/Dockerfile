
# How to run this?
# First you create the volume on host
#	> docker volume create ipcsocket
# Now we have a volume on host named ipcsocket
# Let's run the server 
#	> docker run  -it --ipc shareable -v ipcsocket:/tmp moimhossain/netcore-unixdomainsocket:latest
# Now we will run the client container
#	> docker run  -it --ipc "container:0122efc3a103" -v ipcsocket:/tmp moimhossain/netcore-unixdomainsocket:latest
# Remark: 
# Most

FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["./UnixDomainSocket.Sample.csproj", "./"]
RUN dotnet restore "./UnixDomainSocket.Sample.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "UnixDomainSocket.Sample.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "UnixDomainSocket.Sample.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "UnixDomainSocket.Sample.dll"]