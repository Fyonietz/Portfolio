# Controllers Audit

This document summarizes controllers under `Controllers/`, their routes, behavior, issues, and suggested fixes.

**AuthController (`Controllers/AuthController.cs`)**
- Route group: `/api/v1/auth`
- Endpoint: `POST /register` (form-data)
- Flow:
  - Checks if registration is allowed via `AuthServices.Registered()`.
  - Accepts a posted image, saves to `wwwroot/uploads` and stores filename.
  - Hashes password via `IPasswordService`.
  - Constructs a `User` model and calls `AuthServices.Register(user)`.
  - Calls `IJWTService.GenerateToken(user)` but does not return the token to the client — currently returns `Results.Ok()` with no payload.
- Issues:
  - `AuthServices.Register` SQL parameter ordering bug (see services audit).
  - `Register` does not return inserted id; `user.Id` remains unset before token generation.
  - Token is generated but never returned to the caller.

**ProfileController (`Controllers/ProfileController.cs`)**
- Route group: `/api/v1/profile` (authorization required)
- Endpoints:
  - `GET /api/v1/profile` — returns the first `Profile` row via `QueryFirstOrDefaultAsync<Profile>`.
  - `PUT /api/v1/profile` — updates the single profile row (no id route param). It queries `SELECT Id FROM Profile LIMIT 1` and updates that Id.
- Notes:
  - Uses parameterized queries and Dapper; good async usage.
  - Returns `Results.NotFound()` if no profile row exists for updates.
  - `Updated_At` set to `DateTime.UtcNow` on update.

**ContactController (`Controllers/ContactController.cs`)**
- Route group: `/api/v1/contact` (authorization required)
- Endpoints:
  - `GET /api/v1/contact` — returns all `Contact` rows ordered by `Sort_Order`.
  - `POST /api/v1/contact` — inserts a new contact and returns `Created` with inserted id (uses `SELECT last_insert_rowid()` — SQLite-specific behavior).
  - `PUT /api/v1/contact/{id}` — updates contact by id.
  - `DELETE /api/v1/contact/{id}` — deletes contact by id and returns `NoContent`.
- Notes:
  - No FK to `Profile` as requested.
  - All queries are parameterized; good.

**Recommendations (controllers)**
- Return the generated JWT token (and optionally user info) from `POST /api/v1/auth/register`.
- Ensure `AuthServices.Register` sets and returns the inserted `Id`.
- Add input validation for `Profile` and `Contact` payloads (e.g., required fields, string lengths).
- Consider a `POST /api/v1/profile` upsert endpoint if you want to create the single profile when none exists.
