# -------------------
# Build stage
# -------------------

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /

# Copy the solution and project files
COPY Kafka.Consumer/Kafka.Consumer.csproj /Kafka.Consumer/
COPY *.sln .

# Restore the dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

#Publish the project
RUN dotnet publish Kafka.Consumer/Kafka.Consumer.csproj -c Release -o /app/publish

# -------------------
# Runtime stage
# -------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Kafka.Consumer.dll"]