# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj file and restore dependencies
COPY ["OutReachToursAPI/OutReachToursAPI.csproj", "OutReachToursAPI/"]

# Restore dependencies
RUN dotnet restore "OutReachToursAPI/OutReachToursAPI.csproj"

# Copy remaining source code
COPY . .

# Build and publish
WORKDIR "/src/OutReachToursAPI"
RUN dotnet publish "OutReachToursAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway defaults to port 8080 or the PORT env var
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "OutReachToursAPI.dll"]
