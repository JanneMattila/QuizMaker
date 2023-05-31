#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# 1. Build image(https://hub.docker.com/_/microsoft-dotnet-sdk)
FROM mcr.microsoft.com/dotnet/aspnet:6.0.16-alpine3.17-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0.408-focal-amd64 AS build

WORKDIR /node
ENV NODE_VERSION 18.16.0
RUN apt-get update -qq && \
    apt-get dist-upgrade --no-install-recommends -y && \
    apt-get install -y --no-install-recommends xz-utils

RUN curl -fsSLO --compressed "https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.xz"
RUN tar -xf "/node/node-v$NODE_VERSION-linux-x64.tar.xz"
ENV PATH=$PATH:/node/node-v$NODE_VERSION-linux-x64/bin

WORKDIR /src
COPY "QuizMaker.csproj" .
COPY "package-lock.json" .
COPY "package.json" .
RUN npm install
RUN dotnet restore "QuizMaker.csproj"
COPY . .
RUN dotnet build "QuizMaker.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "QuizMaker.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "QuizMaker.dll"]
