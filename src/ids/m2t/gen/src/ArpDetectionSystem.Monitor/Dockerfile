FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /bld
COPY ["src/ArpDetectionSystem.Monitor/ArpDetectionSystem.Monitor.csproj", "src/ArpDetectionSystem.Monitor/"]
RUN dotnet restore "src/ArpDetectionSystem.Monitor/ArpDetectionSystem.Monitor.csproj"
COPY . .
WORKDIR "/bld/src/ArpDetectionSystem.Monitor"
RUN dotnet build "ArpDetectionSystem.Monitor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ArpDetectionSystem.Monitor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArpDetectionSystem.Monitor.dll"]