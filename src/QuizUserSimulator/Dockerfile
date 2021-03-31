FROM mcr.microsoft.com/dotnet/core/runtime:5.0.201-alpine3.13-amd64 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:5.0.201-focal-amd64 AS build
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
