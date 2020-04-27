FROM mcr.microsoft.com/dotnet/core/runtime:3.1.0-alpine3.10 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.100-buster AS build
WORKDIR /src
COPY "QuizUserSimulator.csproj" .
RUN dotnet restore "QuizUserSimulator.csproj"
COPY . .
RUN dotnet build "QuizUserSimulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuizUserSimulator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuizUserSimulator.dll"]
