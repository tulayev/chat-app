# --------------------------------------------------------------------------------
# Stage 1: Build - Optimize for Docker Caching (Dependency Restore Layer)
# --------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ChatApp.API/ChatApp.API.csproj ChatApp.API/
COPY ChatApp.Application/ChatApp.Application.csproj ChatApp.Application/
COPY ChatApp.Domain/ChatApp.Domain.csproj ChatApp.Domain/
COPY ChatApp.Infrastructure/ChatApp.Infrastructure.csproj ChatApp.Infrastructure/

RUN dotnet restore ChatApp.API/ChatApp.API.csproj

COPY ChatApp.API/ ChatApp.API/
COPY ChatApp.Application/ ChatApp.Application/
COPY ChatApp.Domain/ ChatApp.Domain/
COPY ChatApp.Infrastructure/ ChatApp.Infrastructure/

WORKDIR /src/ChatApp.API
RUN dotnet publish -c Release -o /app/publish

# --------------------------------------------------------------------------------
# Stage 2: Runtime
# --------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ChatApp.API.dll"]
