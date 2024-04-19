FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /bld
COPY ["samples/ArpSample.Monitor/ArpSample.Monitor.csproj", "samples/ArpSample.Monitor/"]
RUN dotnet restore "samples/ArpSample.Monitor/ArpSample.Monitor.csproj"
COPY . .
WORKDIR "/bld/samples/ArpSample.Monitor"
RUN dotnet build "ArpSample.Monitor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ArpSample.Monitor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArpSample.Monitor.dll"]
