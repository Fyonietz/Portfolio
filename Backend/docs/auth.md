# Auth Controller

Route group: `/api/v1/auth`

Endpoints:

- `POST /api/v1/auth/register` (form-data)
  - Fields: `Name`, `Email`, `Password`, optional `Image` (file)
  - Behavior: saves uploaded image to `wwwroot/uploads`, hashes password via `IPasswordService`, inserts into `User` table, generates JWT and returns JSON `{ Token, User }`.
  - Notes: public endpoint (no JWT required).

- `POST /api/v1/auth/login` (application/json)
  - Body: `{ "Email": string, "Password": string }`
  - Behavior: verifies credentials using `AuthServices.Login(...)` (which performs password hash verification via `IPasswordService`), generates a JWT via `IJWTService.GenerateToken(user)`, and returns `200 OK` with a `LoginResponse` containing `Id`, `Token`, `Name`, `Email`, and `ImageUrl`.
  - Errors: returns `404 Not Found` with message `Invalid credentials` on failure.
  - Notes: public endpoint (no JWT required). The JWT must be sent in `Authorization: Bearer <token>` for protected endpoints.

Related services / dependencies:

- `Services/AuthServices.cs` — database access for user registration and login; uses `Services/Database` and `Dapper`.
- `Services/IPasswordService.cs` — wraps `BCrypt` for hashing and verification.
- `Services/IJWTService.cs` — builds tokens using `Env.Value` configuration keys (`Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`).
