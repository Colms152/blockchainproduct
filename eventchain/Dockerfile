#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Development
ENV DBSTRING="Data Source=sqlserver;User ID=SA;Password=OcP2020123;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;"

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["eventchain.csproj", ""]
RUN dotnet restore "./eventchain.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "eventchain.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "eventchain.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eventchain.dll"]
