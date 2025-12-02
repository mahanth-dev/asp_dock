# Stage 1: Build the application
# We use the .NET SDK image. Docker will automatically pull the correct architecture (arm64 on your machine).
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy only the project file first to leverage Docker's layer caching.
COPY ["wepapp/wepapp.csproj", "wepapp/"]

# Restore dependencies for the project
RUN dotnet restore "wepapp/wepapp.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/source/wepapp"

# Build the application
RUN dotnet build "wepapp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "wepapp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Create the final, smaller runtime image
# This will also be the correct arm64 architecture automatically.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "wepapp.dll"]
