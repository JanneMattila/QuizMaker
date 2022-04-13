FROM mcr.microsoft.com/dotnet/runtime:6.0.4-alpine3.15-amd64 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0.202-focal-amd64 AS build
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
