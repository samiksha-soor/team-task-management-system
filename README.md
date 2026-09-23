# Team Task Management System

A task management app where Admins, Managers, and Users each see and do different things.

**Backend:** ASP.NET Core 8 Web API, Entity Framework Core, SQL Server, JWT login
**Frontend:** Angular 18

## Roles

| Role | What they can do |
|---|---|
| Admin | Create teams, pick managers, manage every task and user |
| Manager | Create and assign tasks to their own team, manage their team |
| User | See their own tasks, update their status, add comments |

## Tech Stack
- Backend: .NET 8, ASP.NET Core Web API, EF Core 8, SQL Server, JWT login, BCrypt for passwords, Swagger
- Frontend: Angular 18, Reactive Forms, HttpClient, route guards
- Database: SQL Server, tables created automatically by EF Core

## Project Structure
```
TaskManagementSystem/
├── Backend/
│   ├── TaskManagementSystem.sln       # opens both backend projects in Visual Studio/Rider
│   ├── database/
│   │   ├── schema.sql                 # the tables, for reference (created automatically anyway)
│   │   └── seed.sql                   # optional sample data with ready-to-use logins
│   ├── TaskManagementAPI/             # the Web API
│   │   ├── Controllers/               # Auth, Users, Teams, Tasks, Comments, Notifications, Dashboard
│   │   ├── Models/                    # the database tables as C# classes
│   │   ├── DTOs/                      # what gets sent to and from the API
│   │   ├── Data/                      # the database connection
│   │   ├── Services/                  # login, tokens, notifications
│   │   ├── Middleware/                # turns crashes into clean error responses
│   │   ├── Properties/launchSettings.json
│   │   └── Program.cs                 # where the app starts
│   └── TaskManagementAPI.Tests/       # automated tests
├── Frontend/task-management-ui/       # the Angular app
│   └── src/app/
│       ├── core/                      # shared code: API calls, login guard, JWT handling
│       ├── auth/                      # login, register
│       ├── dashboard/                 # overview page
│       ├── tasks/                     # list, create, view + comments
│       ├── teams/                     # list, create
│       └── shared/navbar/
├── .github/workflows/ci-cd.yml        # runs the build and tests automatically on GitHub
├── docker-compose.yml
└── README.md
```

## Where to start reading
If you're mainly interested in the backend, this order makes sense:
1. `Program.cs` — how the app starts up
2. `Models/User.cs`, `Models/TaskItem.cs` — the two main database tables
3. `Data/ApplicationDbContext.cs` — how the models become SQL tables
4. `Services/AuthService.cs` — login and password handling
5. `Controllers/TasksController.cs` — the rules for who can see and change which tasks
6. `TaskManagementAPI.Tests/` — the tests show how each piece is expected to behave

## Frontend, in plain terms
You don't need deep Angular knowledge to follow this. A few terms that come up a lot:
- **Component** = one screen or piece of UI. Each has a `.ts` file (logic), `.html` file (layout), and `.css` file (styling), e.g. `tasks/task-list/`.
- **Service** (`core/services/*.ts`) = a class that calls the backend API. Components use these instead of calling the API directly.
- **Guard** (`core/guards/*.ts`) = checks before a page loads and can block access, e.g. `authGuard` sends you to `/login` if you're not signed in.
- **Interceptor** (`core/interceptors/jwt.interceptor.ts`) = automatically attaches your login token to every request, so nothing else has to do it manually.

## Setup Instructions

### 1. SQL Server — get a database running first

You need SQL Server running before the API will start. Easiest way is Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name tms-sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```
This starts SQL Server on `localhost:1433`, username `sa`, password `YourStrong@Passw0rd`. Give it about 30 seconds to finish starting up.

Already have SQL Server installed, or using Azure SQL? That works too — just update the connection string in `Backend/TaskManagementAPI/appsettings.json`.

You do **not** need to create the database or tables by hand — the API does this for you the first time it runs (see below).

### 2. Backend (.NET 8)

**You need:** .NET 8 SDK.

```bash
cd Backend/TaskManagementAPI
dotnet restore

# Only needed if your SQL Server isn't at localhost,1433 with sa/YourStrong@Passw0rd -
# otherwise appsettings.json already matches.
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=TaskManagementDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"

dotnet run
```
The first time it runs, the app creates the `TaskManagementDb` database and every table on its own — no separate setup step needed.

The API runs at `https://localhost:7001` (and `http://localhost:5000`). **Swagger opens automatically** in your browser at `https://localhost:7001/swagger`; open that URL yourself if it doesn't.

**Optional — sample data.** Two scripts live in `Backend/database/`:
- `schema.sql` — the same tables the app creates automatically, if you want to look at them or set them up by hand in SQL Server Management Studio / Azure Data Studio.
- `seed.sql` — adds one Admin, one Manager, two Users, a team, and two tasks, with working logins (see "Sample Credentials" below). Run it after the API has started at least once, so the tables already exist:
  ```bash
  docker exec -i tms-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "YourStrong@Passw0rd" -C -i /dev/stdin < Backend/database/seed.sql
  ```
  (No sqlcmd handy? Just paste the contents of `seed.sql` into Azure Data Studio or SSMS and run it there.)

### 3. Frontend (Angular 18)

**You need:** Node.js 18+.

```bash
cd Frontend/task-management-ui
npm install

# Check src/environments/environment.ts points at your backend's URL (default: https://localhost:7001/api)
npm start
```
App runs at `http://localhost:4200`.

### 4. Docker (everything at once)

```bash
docker compose up --build
```
This starts SQL Server, the API, and the Angular app together, already wired up — no manual edits needed. The database and tables are still created automatically on first startup.

## Testing the API in Swagger
Once the backend is running, open `https://localhost:7001/swagger` (it should open by itself). This is the quickest way to try the API without touching the frontend.

1. **Log in.** Open `POST /api/auth/login`, click "Try it out", and send:
   ```json
   { "email": "admin@example.com", "password": "Admin@123" }
   ```
   (These work if you ran `seed.sql`. Otherwise, register a user first via `POST /api/auth/register`, then promote it to Admin — see "Sample Credentials" below.)
   Copy the `token` value from the response.

2. **Authorize.** Click the green **Authorize** button at the top of the page, type `Bearer ` followed by your token, then click Authorize and Close. Every request from here on will include it automatically.

3. **Try things out.** For example:
   - `GET /api/teams` — list teams
   - `POST /api/tasks` — create a task
   - `PATCH /api/tasks/{id}/status` — move a task to `InProgress` or `Done`
   - `GET /api/dashboard/summary` — see the counts
   - `POST /api/tasks/{taskId}/comments` — add a comment

4. **See the roles in action.** Log in again as `manager@example.com` / `Manager@123`, or `user1@example.com` / `User@123`, and notice `GET /api/tasks` returns a smaller, different list each time — and that a `User` gets a `403 Forbidden` if they try `POST /api/tasks`.

Tokens expire after 120 minutes by default — just log in again if yours does.

## Sample Credentials
If you ran `seed.sql`, these work right away:

| Email | Password | Role |
|---|---|---|
| admin@example.com | Admin@123 | Admin |
| manager@example.com | Manager@123 | Manager |
| user1@example.com | User@123 | User |
| user2@example.com | User@123 | User |

Without the seed script, registering a new account always creates a plain `User`. To get an Admin:
1. Register normally.
2. Promote yourself directly in SQL Server:
   ```sql
   UPDATE Users SET Role = 0 WHERE Email = 'your-email@example.com';  -- 0 = Admin
   ```
3. From then on, that Admin can promote others through `PATCH /api/users/{id}/role` instead of touching SQL directly.

## Key Features
- JWT login with roles baked into the token; passwords hashed with BCrypt; tokens expire after a set time
- Access rules enforced on the API and mirrored in the Angular routes
- Tasks with status (`ToDo`/`InProgress`/`Done`), priority, and deadlines
- Teams: Admin creates them and picks a Manager; Managers/Admins add or remove members
- Comments on tasks
- Notifications saved to the database when a task is assigned or its status changes
- Dashboard with totals and an overdue count; task list can be filtered by status, priority, and deadline
- One place (middleware) that turns any unexpected error into a clean JSON response
- Swagger for trying out the API interactively

## API Documentation
Start the API and Swagger opens automatically at `/swagger` (see "Testing the API in Swagger" above). A Postman collection (`postman_collection.json`) is also included.

## Testing
Automated tests live in `Backend/TaskManagementAPI.Tests/`. They use an in-memory database, so they run without a real SQL Server.

Run them:
```bash
cd Backend
dotnet test
```

What's covered:
| Test class | What it checks |
|---|---|
| `AuthServiceTests` | Passwords are hashed, not stored as plain text; duplicate emails are rejected; login only works with the right password |
| `TokenServiceTests` | Generated tokens are well-formed and carry the user's role |
| `NotificationServiceTests` | A notification is created when a task is assigned or its status changes |
| `TasksControllerTests` | The core access rules: Admin sees everything, a Manager sees only their team, a User sees only their own tasks; a Manager can't create tasks for another team; a User can't change someone else's task |

## CI/CD (GitHub Actions)
`.github/workflows/ci-cd.yml` runs automatically on every push and pull request to `main`:
1. **Backend - Build & Test** — builds the API and runs all the tests; results are downloadable from the Actions run.
2. **Frontend - Build** — installs dependencies and does a production Angular build.
3. **Docker - Build Images** — after both of the above succeed, and only on `main`: builds Docker images for the backend and frontend. There are commented-out steps showing where to add pushing those images to a registry when you're ready to deploy.

Nothing to set up — the workflow file is already in `.github/workflows/`, and GitHub picks it up on its own.

## Deployment Notes
- Backend: deploy as a Docker container to Render/Railway; point the connection string at a managed SQL Server/Azure SQL instance through environment variables.
- Frontend: `ng build --configuration production`, deploy the output to Netlify/Vercel; update `environment.prod.ts` with the live API URL and update the API's allowed CORS origins to match.
