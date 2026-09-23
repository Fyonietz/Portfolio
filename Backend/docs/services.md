# Services Audit

This document summarizes the services under `Services/`, their responsibilities, issues found, and suggested fixes.

**Env (`Services/IEnv.cs`)**
- Purpose: static holder for `IConfiguration` (`Env.Value`). Set in `Program.cs` (`Env.Value = builder.Configuration;`).
- Notes: simple global access to config. Works but is global mutable state — acceptable for small personal projects.

**Database (`Services/Database.cs`)**
- Purpose: provides `connect()` which opens and returns an `SqliteConnection` (uses `Env.Value["Database:connection"]`).
- Usage: callers use `using var conn = db.connect();` and Dapper.
- Style: method name `connect` is lowercase (project mixes casing in method names). Follow existing style when adding code.

**AuthServices (`Services/AuthServices.cs`)**
- Public methods:
  - `Task<bool> Register(User user)` — inserts a user via Dapper.
  - `Task<bool> Registered()` — returns whether any user exists.
- Issues:
  - SQL parameter order bug in `Register`: `INSERT INTO User(Username,Email,Password,ImageUrl) VALUES(@Name,@Password,@Email,@ImageUrl)` — the second and third values are swapped. Should be `VALUES(@Name,@Email,@Password,@ImageUrl)`.
  - `Register` does not return the inserted Id; callers will not see the persisted `Id` (impact: `JWTService.GenerateToken(user)` expects `user.Id`).
  - Ensure the `User` table exists with the expected columns and types.

**PasswordService (`Services/IPasswordService.cs`)**
- Purpose: wraps `BCrypt` for password hashing and verification.
- Methods: `HashPassword`, `VerifyPassword`. Implementation is straightforward.

**JWTService (`Services/IJWTService.cs`)**
- Purpose: `GenerateToken(User user)` builds a JWT with `Sub`, `Name`, and `Email` claims and 7-day expiry.
- Notes:
  - Reads key/issuer/audience from `Env.Value["JWT:Key"]`, `Env.Value["JWT:Issuer"]`, `Env.Value["JWT:Audience"]` (note: `Program.cs` uses `Jwt:Key` casing; configuration keys are typically case-insensitive, but keep casing consistent).
  - `user.Id` should be populated when generating the token; otherwise `Sub` claim may be incorrect.

**Recommendations (services)**
- Fix the SQL parameter ordering in `AuthServices.Register`.
- Return inserted Id from `Register` (e.g., `last_insert_rowid()` for SQLite) and set `user.Id` before generating tokens.
- Consider renaming `Database.connect()` to `Connect()` if you prefer consistent casing across methods — but mirror existing style when editing other files.
- Standardize config key casing (`Jwt` vs `JWT`) across `Program.cs` and `JWTService`.
