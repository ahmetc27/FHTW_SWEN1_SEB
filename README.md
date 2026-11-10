# Sports Exercise Battle (SEB)

An HTTP server written in C# **without a web framework**, built for the course Software Engineering 1
at FH Technikum Wien. Users register, record training sessions and compete in tournaments, ranked by
an Elo score.

## Features

- User registration and login with token-based sessions
- Profile management (name, bio, image)
- Recording of training sessions and personal statistics
- Tournaments with participants and winner determination
- Elo-based ranking and scoreboard
- Consistent error handling through custom exceptions

## Architecture

The project is built in layers, each with one responsibility:

| Layer | Responsibility |
|-------|----------------|
| `Http` | Own HTTP implementation: `Server` (TcpListener), `Router`, `Request`, `Response` |
| `Services` | Business logic and validation |
| `Repositories` | Database access with plain SQL |
| `Models` / `Dtos` / `Mappers` | Domain objects and data transfer |
| `Exceptions` | Custom exceptions mapped to HTTP status codes |

Dependencies are wired by hand in `Program.cs` against interfaces (`IUserRepository`, `IUserService`, …).
That keeps the services independent from the database and testable in isolation.

No ASP.NET and no ORM are used — HTTP parsing, routing and SQL are implemented directly.

## Tech Stack

- **C#** / **.NET 9**
- **PostgreSQL 15** – accessed with Npgsql and plain SQL
- **NUnit** – unit tests
- **Docker Compose** – database setup

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| POST | `/users` | Register a new user |
| POST | `/sessions` | Log in and receive a token |
| GET | `/users/{username}` | Get a user profile |
| PUT | `/users/{username}` | Update name, bio and image |
| POST | `/history` | Record a training session |
| GET | `/history` | List own training sessions |
| GET | `/stats` | Personal statistics |
| GET | `/score` | Scoreboard of all users |
| GET | `/tournament` | Current tournament |

Protected endpoints expect the header `Authorization: Basic <username>-sebToken`.
Example requests for every endpoint are collected in `curl.txt`.

## Prerequisites

- .NET 9 SDK
- Docker

## Setup

**1. Start the database**

```bash
docker compose up -d
```

The container creates the database and loads `schema.sql` automatically.

**2. Start the server**

```bash
dotnet run --project SEB
```

The server listens on `http://localhost:10001`.

## Tests

```bash
dotnet test
```

The suite covers users, sessions, statistics, history and tournaments. The services are tested
against mocked repositories, so no database is required to run them.

## Known limitations

This is a university project and reflects the scope of the course:

- Passwords are stored in plain text. Hashing was not part of the assignment — in a later project
  ([job-application-tracker](https://github.com/ahmetc27/job-application-tracker)) passwords are
  hashed with bcrypt.
- The session token is derived from the username instead of being a signed token.
- Database credentials are hard-coded for the local Docker setup.
