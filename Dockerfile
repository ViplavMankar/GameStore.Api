# Use official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY *.sln ./
COPY GameStore.Api/*.csproj GameStore.Api/
RUN dotnet restore GameStore.Api/GameStore.Api.csproj

# Copy the remaining source code and build the app
COPY GameStore.Api/ GameStore.Api/
WORKDIR /app/GameStore.Api
RUN dotnet publish -c Release -o /app/out

# Use the runtime image for final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port Render will use
ENV PORT=8080
EXPOSE 8080

# Start the application
CMD ["dotnet", "GameStore.Api.dll"]
