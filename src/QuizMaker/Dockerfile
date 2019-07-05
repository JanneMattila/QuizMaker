FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/QuizMaker/QuizMaker.csproj", "src/QuizMaker/"]
RUN dotnet restore "src/QuizMaker/QuizMaker.csproj"
COPY . .
WORKDIR "/src/src/QuizMaker"
RUN dotnet build "QuizMaker.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "QuizMaker.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "QuizMaker.dll"]