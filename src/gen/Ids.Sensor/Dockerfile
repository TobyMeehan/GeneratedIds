FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /bld
COPY ["src/Ids.Sensor/Ids.Sensor.csproj", "src/Ids.Sensor/"]
RUN dotnet restore "src/Ids.Sensor/Ids.Sensor.csproj"
COPY . .
WORKDIR "/bld/src/Ids.Sensor"
RUN dotnet build "Ids.Sensor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ids.Sensor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ids.Sensor.dll"]