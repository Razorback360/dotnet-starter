# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["CarDealer.Api/CarDealer.Api.csproj", "CarDealer.Api/"]
RUN dotnet restore "CarDealer.Api/CarDealer.Api.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/CarDealer.Api"

# Build the application
RUN dotnet build "CarDealer.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CarDealer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 9 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published app
COPY --from=publish /app/publish .

# Create directories for logs and database
RUN mkdir -p /app/logs /app/data

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "CarDealer.Api.dll"]
