# API Endpoints

## Profile

- GET `/api/v1/profile` — returns the single `Profile` row (first or default).
- PUT `/api/v1/profile` — updates the single `Profile` row. Body: full `Profile` object.

## Contact

- GET `/api/v1/contact` — returns list of `Contact` ordered by `Sort_Order`.
- POST `/api/v1/contact` — create a `Contact`. Body: `Platform`, `Value`, `Sort_Order`.
- PUT `/api/v1/contact/{id}` — update contact by `id`.
- DELETE `/api/v1/contact/{id}` — delete contact by `id`.

All endpoints require JWT authorization.
