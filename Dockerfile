# Use the official .NET 8 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

#Install LibreOffice + fonts
RUN apt-get update && apt-get install -y \
    libreoffice \
    libreoffice-writer \
    fonts-dejavu \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Expose the port that Render provides
EXPOSE ${PORT}

# Use the .NET 8 SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["GameStore.Api.csproj", "./"]
RUN dotnet restore "GameStore.Api.csproj"

# Create /app/data directory and set permissions
# RUN mkdir -p /app/data && chmod -R 777 /app/data

# Copy all source files and publish the app
COPY . .
RUN dotnet publish "GameStore.Api.csproj" -c Release -o /app/publish

# Build the runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the URL to listen on the port provided by Render
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

# Run the application
ENTRYPOINT ["dotnet", "GameStore.Api.dll"]