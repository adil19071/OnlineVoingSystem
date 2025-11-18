# ==========================
# 1. Build Stage
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire project and build it
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# ==========================
# 2. Runtime Stage
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose Render default port
EXPOSE 10000

# Tell ASP.NET Core to listen on port 10000 (Render requirement)
ENV ASPNETCORE_URLS=http://0.0.0.0:10000

# Run the app
ENTRYPOINT ["dotnet", "OnlineVotingSystem.dll"]
  
