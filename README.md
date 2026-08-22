# Chat App

A real-time chat application with a .NET 8+ backend built on **Clean Architecture** and an Angular 20 SPA client. The backend exposes a REST API plus a SignalR hub for live messaging, and the frontend is a standalone-components Angular app with NgRx for state management.

## Features

- User registration, login, and email verification
- JWT-based authentication (also supported over SignalR via query-string token)
- Real-time messaging through a SignalR hub — REST writes fan out to connected clients in the same request
- Avatar/image uploads via Cloudinary
- Dockerized full stack: API, client, PostgreSQL, and Redis

## Tech Stack

**Backend**
- .NET 8+, ASP.NET Core Web API, SignalR
- Clean Architecture (Domain / Application / Infrastructure / API)
- MediatR (CQRS), FluentValidation, Mapster
- EF Core + PostgreSQL, ASP.NET Identity Core
- Redis
- Cloudinary (image storage), SMTP (email)

**Frontend**
- Angular 20 (standalone components)
- NgRx (Store + Effects)
- Tailwind CSS
- `@microsoft/signalr` client
- Lucide icons, ngx-toastr

## Getting Started

### Docker (recommended)

Run from `src/`. Copy `.env.example` to `.env` and fill in the required secrets (connection string, Redis, `JwtTokenKey`, Cloudinary, SMTP):

```bash
cp .env.example .env

# Development (source-mounted volumes, live reload)
docker compose -f docker-compose.dev.yml up --build

# Production-like (built images)
docker compose up --build
```

This brings up:
- `api` — http://localhost:5000
- `client` — http://localhost:4200
- `postgres:17` — 5432
- `redis:7.2` — 6379

Database migrations are applied automatically at API startup — no manual migration step needed.

### Manual setup

**Backend** (run from `src/`, requires PostgreSQL and Redis reachable):

```bash
dotnet build ChatApp.sln
dotnet run --project ChatApp.API
```

**Frontend** (run from `src/ChatApp.Client/`):

```bash
npm install
npm run start:dev
```

## Useful Commands

### Backend

```bash
dotnet build ChatApp.sln
dotnet run --project ChatApp.API
dotnet ef migrations add <Name> -p ChatApp.Infrastructure -s ChatApp.API
dotnet ef database update -p ChatApp.Infrastructure -s ChatApp.API
```

### Frontend

```bash
npm run start:dev     # ng serve --c=development
npm run build:dev     # ng build --c=development
npm run build:prod    # ng build --c=production
npm test              # ng test (Karma/Jasmine)
```

## Project Structure

```
src/
├── ChatApp.Domain/          # Entities & core abstractions
├── ChatApp.Application/     # CQRS commands/queries, validation, SignalR hub
├── ChatApp.Infrastructure/  # EF Core, Identity, Redis, service implementations
├── ChatApp.API/             # ASP.NET Core host (controllers, auth, middleware)
├── ChatApp.Client/          # Angular 20 SPA
├── docker-compose.yml
├── docker-compose.dev.yml
└── .env.example
```
