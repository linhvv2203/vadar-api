#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["VADAR.SyslogWorker/VADAR.SyslogWorker.csproj", "VADAR.SyslogWorker/"]
RUN dotnet restore "VADAR.SyslogWorker/VADAR.SyslogWorker.csproj"
COPY . .
WORKDIR "/src/VADAR.SyslogWorker"
RUN dotnet build "VADAR.SyslogWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VADAR.SyslogWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VADAR.SyslogWorker.dll"]