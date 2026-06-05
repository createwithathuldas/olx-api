# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["olx-api.csproj", "./"]
RUN dotnet restore "olx-api.csproj"

# Copy the rest of the source code and build
COPY . .
RUN dotnet build "olx-api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "olx-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set dynamic port configuration
ENV ASPNETCORE_URLS=http://+:5141
EXPOSE 5141

ENTRYPOINT ["dotnet", "olx-api.dll"]
