# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

ChatApp is a real-time chat application: a .NET 8+ backend (Clean Architecture) exposing a REST API + SignalR hub, and an Angular 20 SPA client. All source lives under `src/`.

## Commands

### Backend (.NET) — run from `src/`

```
dotnet build ChatApp.sln              # build all projects
dotnet run --project ChatApp.API      # run the API locally (needs Postgres/Redis reachable)
dotnet ef migrations add <Name> -p ChatApp.Infrastructure -s ChatApp.API   # add an EF Core migration
dotnet ef database update -p ChatApp.Infrastructure -s ChatApp.API        # apply migrations
```

There are no test projects in the solution currently.

Migrations are applied automatically at startup via `app.MigrateDatabaseAsync()` in `ChatApp.API/Program.cs` (see `WebApplicationExtensions.cs`), so a fresh container/environment self-migrates on boot — no manual `database update` step needed in Docker.

### Frontend (Angular) — run from `src/ChatApp.Client/`

```
npm run start:dev     # ng serve --c=development (dev config, host 0.0.0.0)
npm run build:dev      # ng build --c=development
npm run build:prod     # ng build --c=production
npm test                # ng test (Karma/Jasmine)
```

To run a single spec file, use Angular CLI's `--include` flag, e.g. `ng test --include='**/login.component.spec.ts'`.

### Docker (full stack) — run from `src/`

```
docker compose -f docker-compose.dev.yml up --build   # dev: source-mounted volumes, live reload
docker compose up --build                              # prod-like: built images, no source mounts
```

Both compose files bring up `api` (port 5000→8080), `client` (port 4200), `postgres:17` (5432), and `redis:7.2` (6379). Backend config secrets are supplied via `src/.env` (see `src/.env.example` for the required keys — connection string, Redis host/port, `JwtTokenKey`, Cloudinary and SMTP credentials).

CI (`.github/workflows/deploy.yml`) builds and pushes `umarbek/chatapp-api` and `umarbek/chatapp-client` images to Docker Hub on push to `main`.

## Architecture

### Backend: Clean Architecture, 4 projects (see `src/ChatApp.sln`)

- **ChatApp.Domain** — POCO entities only (`AppUser`, `Chat`, `Message`) and abstractions (`IAuditableEntity`). No dependencies on other layers.
- **ChatApp.Application** — business logic, framework-agnostic except for MediatR/FluentValidation/Mapster:
  - **CQRS** (`CQRS/<Feature>/Commands|Queries|Handlers`) — MediatR commands/queries per feature (`Login`, `Register`, `EmailVerification`, `Messages`). Every handler returns `ApiResponse<T>` (see `Helpers/ApiResponse.cs`), a uniform success/error envelope consumed by the API layer.
  - **Common/Behaviors/ValidationBehavior** — a MediatR pipeline behavior that runs FluentValidation validators (from `Validators/`) before every command/query handler executes.
  - **Common/Interfaces** — abstractions for infrastructure concerns (`IUnitOfWork`, `IJwtTokenService`, `IImageStoreService`, `IEmailSenderService`, `IVerificationCodeService`), implemented in Infrastructure and injected via DI.
  - **Hubs/ChatHub** — SignalR hub; handles group membership per chat (`JoinChat`/`LeaveChat` join/leave a `chat-{chatId}` group). Command handlers push real-time updates by resolving `IHubContext<ChatHub>` and calling `Clients.Group(...)`, so a REST write (e.g. `SendMessageCommandHandler`) also fans out over the socket in the same request — REST and realtime are not separate code paths.
  - **Mappings** — Mapster `IRegister` profiles, scanned automatically at startup (no per-mapping registration needed).
- **ChatApp.Infrastructure** — EF Core (`ChatAppDbContext`), a generic `UnitOfWork` (single generic repository over `IQueryable<TEntity>`, no per-entity repositories), Identity Core for user/password management, Redis connection multiplexer, and concrete implementations of the Application-layer service interfaces (JWT issuing, Cloudinary image storage, SMTP email, verification codes). All registered in `DependencyInjection.AddInfrastructure`.
- **ChatApp.API** — ASP.NET Core host: controllers (`Controllers/`), JWT bearer auth (also accepts the token via `?access_token=` query string for SignalR hub connections — see `AddInfrastructure`'s `OnMessageReceived`), global `ExceptionMiddleware`, NLog file logging (`Logs/`), and CORS locked to `http://localhost:4200`.

Layer dependency direction: API → Infrastructure → Application → Domain (Infrastructure implements interfaces defined in Application; Domain has no outward dependencies).

### Frontend: Angular 20, standalone components + NgRx

- Path aliases (see `tsconfig.json`): `@app/*`, `@core/*`, `@models`, `@pages/*`, `@shared/*`, `@store/*`. Use these instead of relative `../../` imports.
- **core/** — cross-cutting singletons: `guards/auth.guard.ts`, HTTP `interceptors/` (`jwtInterceptor` attaches the bearer token from `AuthService`; `errorInterceptor` handles HTTP errors), and `services/` (`AuthService`, `ChatService`, `EmailVerificationService`).
- **store/** — NgRx state, currently scoped to `chat` (`chat.state.ts`, `.actions.ts`, `.reducer.ts`, `.effects.ts`, `.selector.ts`) registered in `app.config.ts` via `provideStore`/`provideEffects`. Only `currentChat` lives in the store today — most page state is local component state.
- **pages/** — routed feature areas: `auth` (login/register/verify-email, each with its own `.component.ts`), `chat`.
- Global providers are wired in `app.config.ts`: NgRx store/effects, HTTP client with the two interceptors above, `ToastrModule` for notifications, async animations.
- Real-time messaging on the client uses `@microsoft/signalr`, connecting to the API's `hubs/chat` endpoint mapped in `Program.cs`.

### Cross-cutting conventions

- Every API response (from MediatR handlers) is wrapped in `ApiResponse<T>` — check this envelope shape when adding new commands/queries or when consuming API responses from the Angular services.
- New backend features follow the CQRS folder pattern: add `Commands`/`Queries` + `Handlers` under `CQRS/<Feature>/`, a validator under `Validators/` if input needs validation, and a Mapster mapping under `Mappings/` if a new entity↔DTO mapping is needed — DI wiring for validators/handlers/mappings is automatic via assembly scanning, no manual registration required.
- Data access always goes through `IUnitOfWork` (generic `GetQueryable<T>`/`AddAsync`/`Update`/`Delete`), not injected `DbContext` or per-entity repositories.
