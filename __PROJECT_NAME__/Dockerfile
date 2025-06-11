# -------------------
# Build stage
# -------------------

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /

# Copy the solution and project files
COPY __PROJECT_NAME__/__PROJECT_NAME__.csproj /__PROJECT_NAME__/
COPY *.scproj .

# Restore the dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

#Publish the project
RUN dotnet publish __PROJECT_NAME__/__PROJECT_NAME__.csproj -c Release -o /app/publish

# -------------------
# Runtime stage
# -------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "__PROJECT_NAME__.dll"]