#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["VADAR.Worker/VADAR.Worker.csproj", "VADAR.Worker/"]
RUN dotnet restore "VADAR.Worker/VADAR.Worker.csproj"
COPY . .
WORKDIR "/src/VADAR.Worker"
RUN dotnet build "VADAR.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VADAR.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VADAR.Worker.dll"]