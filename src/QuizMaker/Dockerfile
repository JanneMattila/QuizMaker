FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview9-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview9-buster AS build

WORKDIR /node
ENV NODE_VERSION 12.9.1
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
