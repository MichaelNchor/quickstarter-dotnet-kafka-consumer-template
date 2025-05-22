# -------------------
# Build stage
# -------------------

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY src/Kafka.Consumer/Kafka.Consumer.csproj src/Kafka.Consumer/
COPY tests/Kafka.Consumer.Tests/Kafka.Consumer.Tests.csproj tests/Kafka.Consumer.Tests/
COPY *.sln .

# Restore the dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

#Publish the project
RUN dotnet publish src/Kafka.Consumer/Kafka.Consumer.csproj -c Release -o /app/publish

# -------------------
# Runtime stage
# -------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Kafka.Consumer.dll"]