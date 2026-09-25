# API Contract Documentation

Dokumen ini berfungsi sebagai contract backend untuk tim frontend. Fokus audit mencakup seluruh endpoint yang tersedia pada backend .NET Minimal API dengan SQLite, Dapper, JWT, dan file upload multipart.

## Catatan umum

- Prefix route: `/api/v1`
- JWT diset untuk role `Admin` pada saat token dibuat.
- Endpoint admin memerlukan header `Authorization: Bearer <token>`.
- Pada payload JSON, ASP.NET Core akan meng-serialize properti C# ke camelCase secara default. Contoh: `Role_Title` dikirim/diterima sebagai `roleTitle` pada JSON, sementara multipart form field tetap memakai nama asli seperti `Role_Title`.
- Beberapa field di multipart/form-data memakai nama yang terlihat seperti snake_case / PascalCase; field-name ini bersifat case-sensitive dan harus dikirim persis seperti yang diharapkan backend.
- Terdapat beberapa inkonsistensi naming yang perlu diperhatikan oleh frontend:
  - `Role_Title` / `Photo_Url` / `Short_Description` / `TechStack_Ids` menggunakan underscore di model C# dan form.
  - JSON response biasanya muncul dalam camelCase, misalnya `photoUrl`, `shortDescription`, `techStackIds`.
  - `Project.Photos` dan `Project.TechStack_Ids` secara semantik adalah array, tetapi di backend disimpan sebagai string CSV (`"/uploads/projects/a.jpg,/uploads/projects/b.jpg"`) atau string `"1,2,3"`.
  - `LoginResponse` mengandung properti `password` dengan nilai `""` (empty string), padahal seharusnya tidak dikirim ke client.

---

## Auth

### POST /api/v1/auth/register

**Auth:** Public

**Request:**
- Format: multipart/form-data
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| Name | string | ✓ | Nama user yang akan dibuat |
| Email | string | ✓ | Email user, unik |
| Password | string | ✓ | Password plaintext; akan di-hash di backend |
| Image | file | ✗ | File foto profil opsional; disimpan ke `wwwroot/uploads` |

**Response sukses:**
- Status: 200
- Body:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Alice",
    "email": "alice@example.com",
    "imageUrl": "/uploads/abc123.png"
  }
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 400 | `Registration Is Disabled.` atau `Register failed.` |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const form = new FormData();
form.append('Name', 'Alice');
form.append('Email', 'alice@example.com');
form.append('Password', 'secret123');
form.append('Image', fileInput.files[0]);

const res = await fetch('http://localhost:5000/api/v1/auth/register', {
  method: 'POST',
  body: form
});
const data = await res.json();
```

---

### POST /api/v1/auth/login

**Auth:** Public

**Request:**
- Format: JSON
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| email | string | ✓ | Email user |
| password | string | ✓ | Password user |

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "name": "Alice",
  "email": "alice@example.com",
  "imageUrl": "/uploads/abc123.png",
  "password": ""
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 404 | `Invalid credentials` |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'alice@example.com',
    password: 'secret123'
  })
});
const data = await res.json();
```

---

## Profile

### GET /api/v1/profile/

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
[
  {
    "id": 1,
    "name": "John Doe",
    "roleTitle": "Frontend Engineer",
    "description": "Full-stack developer focused on modern web apps",
    "photoUrl": "/uploads/profile/abc123.png",
    "status": "Available",
    "bio": "I build products for startups and enterprises",
    "updatedAt": "2026-09-25T09:00:00Z"
  }
]
```

**Response error:**
| Status | Kondisi |
|---|---|
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/profile/');
const profiles = await res.json();
```

---

### GET /api/v1/profile/{id}

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "name": "John Doe",
  "roleTitle": "Frontend Engineer",
  "description": "Full-stack developer focused on modern web apps",
  "photoUrl": "/uploads/profile/abc123.png",
  "status": "Available",
  "bio": "I build products for startups and enterprises",
  "updatedAt": "2026-09-25T09:00:00Z"
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/profile/1');
const profile = await res.json();
```

---

### POST /api/v1/profile/

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: multipart/form-data
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| Name | string | ✓ | Nama profil |
| Role_Title | string | ✓ | Jabatan / peran |
| Description | string | ✓ | Deskripsi singkat |
| Status | string | ✓ | Status publik profil |
| Bio | string | ✓ | Bio panjang |
| file | file | ✗ | Foto profil; jika ada, backend menyimpan sebagai `Photo_Url` |

**Response sukses:**
- Status: 201
- Body:
```json
{
  "id": 1,
  "name": "John Doe",
  "roleTitle": "Frontend Engineer",
  "description": "Full-stack developer focused on modern web apps",
  "photoUrl": "/uploads/profile/abc123.png",
  "status": "Available",
  "bio": "I build products for startups and enterprises",
  "updatedAt": "2026-09-25T09:00:00Z"
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 400 | Validasi gagal / `Failed to create profile.` |
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const form = new FormData();
form.append('Name', 'John Doe');
form.append('Role_Title', 'Frontend Engineer');
form.append('Description', 'Full-stack developer focused on modern web apps');
form.append('Status', 'Available');
form.append('Bio', 'I build products for startups and enterprises');
form.append('file', fileInput.files[0]);

const res = await fetch('http://localhost:5000/api/v1/profile/', {
  method: 'POST',
  headers: { Authorization: `Bearer ${token}` },
  body: form
});
```

---

### PUT /api/v1/profile/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: multipart/form-data
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| Name | string | ✓ | Nama profil |
| Role_Title | string | ✓ | Jabatan / peran |
| Description | string | ✓ | Deskripsi singkat |
| Status | string | ✓ | Status publik profil |
| Bio | string | ✓ | Bio panjang |
| file | file | ✗ | Foto profil baru; jika tidak dikirim, backend mempertahankan `existing.Photo_Url` |

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "name": "John Doe",
  "roleTitle": "Lead Frontend Engineer",
  "description": "Focus on design systems and product UX",
  "photoUrl": "/uploads/profile/updated.png",
  "status": "Available",
  "bio": "I lead product engineering teams",
  "updatedAt": "2026-09-25T12:00:00Z"
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const form = new FormData();
form.append('Name', 'John Doe');
form.append('Role_Title', 'Lead Frontend Engineer');
form.append('Description', 'Focus on design systems and product UX');
form.append('Status', 'Available');
form.append('Bio', 'I lead product engineering teams');
if (fileInput.files[0]) form.append('file', fileInput.files[0]);

const res = await fetch('http://localhost:5000/api/v1/profile/1', {
  method: 'PUT',
  headers: { Authorization: `Bearer ${token}` },
  body: form
});
```

---

### DELETE /api/v1/profile/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 204
- Body: none

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/profile/1', {
  method: 'DELETE',
  headers: { Authorization: `Bearer ${token}` }
});
```

---

## Contact

### GET /api/v1/contact/

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
[
  {
    "id": 1,
    "platform": "GitHub",
    "value": "https://github.com/username",
    "sortOrder": 1
  }
]
```

**Response error:**
| Status | Kondisi |
|---|---|
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/contact/');
const contacts = await res.json();
```

---

### GET /api/v1/contact/{id}

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "platform": "GitHub",
  "value": "https://github.com/username",
  "sortOrder": 1
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/contact/1');
const contact = await res.json();
```

---

### POST /api/v1/contact/

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: JSON
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| platform | string | ✓ | Nama platform kontak |
| value | string | ✓ | Nilai kontak, contoh URL atau username |
| sortOrder | number | ✗ | Urutan tampilan; default `0` |

**Response sukses:**
- Status: 201
- Body:
```json
{
  "id": 1,
  "platform": "GitHub",
  "value": "https://github.com/username",
  "sortOrder": 1
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 400 | Validasi gagal / `Failed to create.` |
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/contact/', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`
  },
  body: JSON.stringify({
    platform: 'GitHub',
    value: 'https://github.com/username',
    sortOrder: 1
  })
});
```

---

### PUT /api/v1/contact/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: JSON
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| platform | string | ✓ | Nama platform kontak |
| value | string | ✓ | Nilai kontak |
| sortOrder | number | ✗ | Urutan tampilan |

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "platform": "LinkedIn",
  "value": "https://linkedin.com/in/username",
  "sortOrder": 2
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/contact/1', {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`
  },
  body: JSON.stringify({
    platform: 'LinkedIn',
    value: 'https://linkedin.com/in/username',
    sortOrder: 2
  })
});
```

---

### DELETE /api/v1/contact/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 204
- Body: none

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/contact/1', {
  method: 'DELETE',
  headers: { Authorization: `Bearer ${token}` }
});
```

---

## Project

### GET /api/v1/project/

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
[
  {
    "id": 1,
    "title": "Portfolio Website",
    "slug": "portfolio-website",
    "shortDescription": "Modern portfolio for startup",
    "fullDescription": "Project details and case study",
    "photos": "/uploads/projects/p1.jpg,/uploads/projects/p2.jpg",
    "repoUrl": "https://github.com/username/repo",
    "demoUrl": "https://demo.example.com",
    "sortOrder": 1,
    "published": true,
    "createdAt": "2026-09-25T09:00:00Z",
    "updatedAt": "2026-09-25T09:00:00Z",
    "techStacks": [
      {
        "id": 1,
        "name": ".NET",
        "iconUrl": "/uploads/tech-stack/net.png",
        "sortOrder": 1
      }
    ]
  }
]
```

**Response error:**
| Status | Kondisi |
|---|---|
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/project/');
const projects = await res.json();
```

---

### GET /api/v1/project/id/{id}

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "title": "Portfolio Website",
  "slug": "portfolio-website",
  "shortDescription": "Modern portfolio for startup",
  "fullDescription": "Project details and case study",
  "photos": "/uploads/projects/p1.jpg,/uploads/projects/p2.jpg",
  "repoUrl": "https://github.com/username/repo",
  "demoUrl": "https://demo.example.com",
  "sortOrder": 1,
  "published": true,
  "createdAt": "2026-09-25T09:00:00Z",
  "updatedAt": "2026-09-25T09:00:00Z",
  "techStacks": []
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/project/id/1');
const project = await res.json();
```

---

### GET /api/v1/project/{slug}

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "title": "Portfolio Website",
  "slug": "portfolio-website",
  "shortDescription": "Modern portfolio for startup",
  "fullDescription": "Project details and case study",
  "photos": "/uploads/projects/p1.jpg,/uploads/projects/p2.jpg",
  "repoUrl": "https://github.com/username/repo",
  "demoUrl": "https://demo.example.com",
  "sortOrder": 1,
  "published": true,
  "createdAt": "2026-09-25T09:00:00Z",
  "updatedAt": "2026-09-25T09:00:00Z",
  "techStacks": [
    { "id": 1, "name": ".NET", "iconUrl": "/uploads/tech-stack/net.png", "sortOrder": 1 }
  ]
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/project/portfolio-website');
const project = await res.json();
```

---

### POST /api/v1/project/

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: multipart/form-data
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| Title | string | ✓ | Judul project |
| Slug | string | ✓ | Slug URL project |
| Short_Description | string | ✓ | Ringkasan singkat |
| Full_Description | string | ✓ | Deskripsi lengkap |
| Repo_Url | string | ✗ | URL repository |
| Demo_Url | string | ✗ | URL demo |
| Sort_Order | number | ✗ | Urutan tampilan; default `0` |
| Published | boolean/string | ✗ | Status publish; backend mem-parsing `bool.TryParse` |
| TechStack_Ids | string | ✗ | CSV dari id tech stack, contoh `"1,2,3"` |
| files | file[] | ✗ | Satu atau lebih foto project; backend menyimpan ke `Photos` sebagai string CSV |

**Response sukses:**
- Status: 201
- Body:
```json
{
  "title": "Portfolio Website",
  "slug": "portfolio-website",
  "shortDescription": "Modern portfolio for startup",
  "fullDescription": "Project details and case study",
  "photos": "/uploads/projects/p1.jpg,/uploads/projects/p2.jpg",
  "repoUrl": "https://github.com/username/repo",
  "demoUrl": "https://demo.example.com",
  "sortOrder": 1,
  "published": true,
  "techStackIds": [1, 2, 3]
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 400 | Validasi gagal / `Failed to create project.` |
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const form = new FormData();
form.append('Title', 'Portfolio Website');
form.append('Slug', 'portfolio-website');
form.append('Short_Description', 'Modern portfolio for startup');
form.append('Full_Description', 'Project details and case study');
form.append('Repo_Url', 'https://github.com/username/repo');
form.append('Demo_Url', 'https://demo.example.com');
form.append('Sort_Order', '1');
form.append('Published', 'true');
form.append('TechStack_Ids', '1,2,3');
if (files) files.forEach(file => form.append('files', file));

const res = await fetch('http://localhost:5000/api/v1/project/', {
  method: 'POST',
  headers: { Authorization: `Bearer ${token}` },
  body: form
});
```

---

### PUT /api/v1/project/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: multipart/form-data
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| Title | string | ✓ | Judul project |
| Slug | string | ✓ | Slug URL project |
| Short_Description | string | ✓ | Ringkasan singkat |
| Full_Description | string | ✓ | Deskripsi lengkap |
| Repo_Url | string | ✗ | URL repository |
| Demo_Url | string | ✗ | URL demo |
| Sort_Order | number | ✗ | Urutan tampilan |
| Published | boolean/string | ✗ | Status publish |
| TechStack_Ids | string | ✗ | CSV dari id tech stack, contoh `"1,2,3"` |
| files | file[] | ✗ | Upload foto baru; jika tidak dikirim, backend memakai foto lama |

**Response sukses:**
- Status: 200
- Body:
```json
{
  "title": "Portfolio Website",
  "slug": "portfolio-website",
  "shortDescription": "Modern portfolio for startup",
  "fullDescription": "Project details and case study",
  "photos": "/uploads/projects/p1.jpg,/uploads/projects/p2.jpg",
  "repoUrl": "https://github.com/username/repo",
  "demoUrl": "https://demo.example.com",
  "sortOrder": 1,
  "published": true,
  "techStackIds": [1, 2, 3]
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const form = new FormData();
form.append('Title', 'Portfolio Website');
form.append('Slug', 'portfolio-website');
form.append('Short_Description', 'Modern portfolio for startup');
form.append('Full_Description', 'Project details and case study');
form.append('TechStack_Ids', '1,2,3');
if (newFiles.length) newFiles.forEach(file => form.append('files', file));

const res = await fetch('http://localhost:5000/api/v1/project/1', {
  method: 'PUT',
  headers: { Authorization: `Bearer ${token}` },
  body: form
});
```

---

### DELETE /api/v1/project/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 204
- Body: none

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/project/1', {
  method: 'DELETE',
  headers: { Authorization: `Bearer ${token}` }
});
```

---

## TechStack

### GET /api/v1/tech-stack/

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
[
  {
    "id": 1,
    "name": ".NET",
    "iconUrl": "/uploads/tech-stack/net.png",
    "sortOrder": 1
  }
]
```

**Response error:**
| Status | Kondisi |
|---|---|
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/tech-stack/');
const techStacks = await res.json();
```

---

### GET /api/v1/tech-stack/{id}

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "name": ".NET",
  "iconUrl": "/uploads/tech-stack/net.png",
  "sortOrder": 1
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/tech-stack/1');
const tech = await res.json();
```

---

### POST /api/v1/tech-stack/

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: multipart/form-data
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| Name | string | ✓ | Nama teknologi |
| Sort_Order | number | ✗ | Urutan tampilan |
| file | file | ✗ | Ikon teknologi; disimpan sebagai `Icon_Url` |

**Response sukses:**
- Status: 201
- Body:
```json
{
  "id": 1,
  "name": ".NET",
  "iconUrl": "/uploads/tech-stack/net.png",
  "sortOrder": 1
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 400 | Validasi gagal / `Failed to create tech stack.` |
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const form = new FormData();
form.append('Name', '.NET');
form.append('Sort_Order', '1');
form.append('file', fileInput.files[0]);

const res = await fetch('http://localhost:5000/api/v1/tech-stack/', {
  method: 'POST',
  headers: { Authorization: `Bearer ${token}` },
  body: form
});
```

---

### PUT /api/v1/tech-stack/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: multipart/form-data
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| Name | string | ✓ | Nama teknologi |
| Sort_Order | number | ✗ | Urutan tampilan |
| file | file | ✗ | Ikon baru; jika tidak dikirim, backend memakai `existing.Icon_Url` |

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "name": ".NET 8",
  "iconUrl": "/uploads/tech-stack/net-8.png",
  "sortOrder": 2
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const form = new FormData();
form.append('Name', '.NET 8');
form.append('Sort_Order', '2');
if (fileInput.files[0]) form.append('file', fileInput.files[0]);

const res = await fetch('http://localhost:5000/api/v1/tech-stack/1', {
  method: 'PUT',
  headers: { Authorization: `Bearer ${token}` },
  body: form
});
```

---

### DELETE /api/v1/tech-stack/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 204
- Body: none

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/tech-stack/1', {
  method: 'DELETE',
  headers: { Authorization: `Bearer ${token}` }
});
```

---

## Skill

### GET /api/v1/skill/

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
[
  {
    "id": 1,
    "name": "JavaScript",
    "category": "Frontend",
    "description": "Interactive UI and logic",
    "iconUrl": "/uploads/tech-stack/js.png",
    "level": 90,
    "sortOrder": 1,
    "published": true,
    "createdAt": "2026-09-25T09:00:00Z",
    "updatedAt": "2026-09-25T09:00:00Z"
  }
]
```

**Response error:**
| Status | Kondisi |
|---|---|
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/skill/');
const skills = await res.json();
```

---

### GET /api/v1/skill/{id}

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "name": "JavaScript",
  "category": "Frontend",
  "description": "Interactive UI and logic",
  "iconUrl": "/uploads/tech-stack/js.png",
  "level": 90,
  "sortOrder": 1,
  "published": true,
  "createdAt": "2026-09-25T09:00:00Z",
  "updatedAt": "2026-09-25T09:00:00Z"
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/skill/1');
const skill = await res.json();
```

---

### POST /api/v1/skill/

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: JSON
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| name | string | ✓ | Nama skill |
| category | string | ✓ | Kategori skill |
| description | string | ✓ | Deskripsi skill |
| iconUrl | string | ✗ | URL ikon skill |
| level | number | ✗ | Tingkat skill, contoh `90` |
| sortOrder | number | ✗ | Urutan tampilan |
| published | boolean | ✗ | Status tampil; default `true` |

**Response sukses:**
- Status: 201
- Body:
```json
{
  "id": 1,
  "name": "JavaScript",
  "category": "Frontend",
  "description": "Interactive UI and logic",
  "iconUrl": "/uploads/tech-stack/js.png",
  "level": 90,
  "sortOrder": 1,
  "published": true,
  "createdAt": "2026-09-25T09:00:00Z",
  "updatedAt": "2026-09-25T09:00:00Z"
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 400 | Validasi gagal / `Failed to create skill.` |
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/skill/', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`
  },
  body: JSON.stringify({
    name: 'JavaScript',
    category: 'Frontend',
    description: 'Interactive UI and logic',
    iconUrl: '/uploads/tech-stack/js.png',
    level: 90,
    sortOrder: 1,
    published: true
  })
});
```

---

### PUT /api/v1/skill/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: JSON
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| name | string | ✓ | Nama skill |
| category | string | ✓ | Kategori skill |
| description | string | ✓ | Deskripsi skill |
| iconUrl | string | ✗ | URL ikon skill |
| level | number | ✗ | Tingkat skill |
| sortOrder | number | ✗ | Urutan tampilan |
| published | boolean | ✗ | Status tampil |

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "name": "JavaScript",
  "category": "Frontend",
  "description": "Interactive UI and logic",
  "iconUrl": "/uploads/tech-stack/js.png",
  "level": 95,
  "sortOrder": 1,
  "published": true,
  "createdAt": "2026-09-25T09:00:00Z",
  "updatedAt": "2026-09-25T12:00:00Z"
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/skill/1', {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`
  },
  body: JSON.stringify({
    name: 'JavaScript',
    category: 'Frontend',
    description: 'Interactive UI and logic',
    iconUrl: '/uploads/tech-stack/js.png',
    level: 95,
    sortOrder: 1,
    published: true
  })
});
```

---

### DELETE /api/v1/skill/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 204
- Body: none

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/skill/1', {
  method: 'DELETE',
  headers: { Authorization: `Bearer ${token}` }
});
```

---

## Achievement

### GET /api/v1/achievement/

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
[
  {
    "id": 1,
    "title": "Best Startup Award",
    "description": "Top 10 startup in the region",
    "achievedAt": "2025-12-01",
    "sortOrder": 1
  }
]
```

**Response error:**
| Status | Kondisi |
|---|---|
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/achievement/');
const achievements = await res.json();
```

---

### GET /api/v1/achievement/{id}

**Auth:** Public

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "title": "Best Startup Award",
  "description": "Top 10 startup in the region",
  "achievedAt": "2025-12-01",
  "sortOrder": 1
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/achievement/1');
const achievement = await res.json();
```

---

### POST /api/v1/achievement/

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: JSON
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| title | string | ✓ | Judul achievement |
| description | string | ✓ | Deskripsi achievement |
| achievedAt | string | ✗ | Tanggal pencapaian, contoh `2025-12-01` |
| sortOrder | number | ✗ | Urutan tampilan |

**Response sukses:**
- Status: 201
- Body:
```json
{
  "id": 1,
  "title": "Best Startup Award",
  "description": "Top 10 startup in the region",
  "achievedAt": "2025-12-01",
  "sortOrder": 1
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 400 | Validasi gagal / `Failed to create.` |
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/achievement/', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`
  },
  body: JSON.stringify({
    title: 'Best Startup Award',
    description: 'Top 10 startup in the region',
    achievedAt: '2025-12-01',
    sortOrder: 1
  })
});
```

---

### PUT /api/v1/achievement/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: JSON
- Body:

| Field | Type | Required | Keterangan |
|---|---|---|---|
| title | string | ✓ | Judul achievement |
| description | string | ✓ | Deskripsi achievement |
| achievedAt | string | ✗ | Tanggal pencapaian |
| sortOrder | number | ✗ | Urutan tampilan |

**Response sukses:**
- Status: 200
- Body:
```json
{
  "id": 1,
  "title": "Best Startup Award",
  "description": "Top 10 startup in the region",
  "achievedAt": "2025-12-02",
  "sortOrder": 2
}
```

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/achievement/1', {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`
  },
  body: JSON.stringify({
    title: 'Best Startup Award',
    description: 'Top 10 startup in the region',
    achievedAt: '2025-12-02',
    sortOrder: 2
  })
});
```

---

### DELETE /api/v1/achievement/{id}

**Auth:** Bearer token (Admin role required)

**Request:**
- Format: Tidak ada body
- Body: none

**Response sukses:**
- Status: 204
- Body: none

**Response error:**
| Status | Kondisi |
|---|---|
| 401 | Tidak ada token atau token invalid |
| 403 | Role tidak sesuai (`Admin` required) |
| 404 | Data tidak ditemukan |
| 500 | Server error |

**Contoh fetch (JavaScript):**
```js
const res = await fetch('http://localhost:5000/api/v1/achievement/1', {
  method: 'DELETE',
  headers: { Authorization: `Bearer ${token}` }
});
```

---

## Ringkasan field naming convention

- Nama model C# kebanyakan mengandung underscore, misalnya: `Role_Title`, `Photo_Url`, `Short_Description`, `TechStack_Ids`, `Created_At`.
- Nama field upload form (multipart) juga mengekspos nama asli dari model/backend, misalnya `Role_Title`, `Short_Description`, `TechStack_Ids`.
- JSON response yang dikirim ke frontend cenderung memakai camelCase karena ASP.NET Core default serializer, misalnya: `roleTitle`, `photoUrl`, `shortDescription`, `createdAt`.
- Untuk `Project`, properti `Photos` dan `TechStack_Ids` tidak dikirim sebagai array murni di JSON; backend mengirim string CSV, sehingga frontend perlu parse manual.
- Untuk `Project`, field `techStacks` berisi object array, sedangkan `TechStack_Ids` hanya dipakai untuk request input.
- Untuk `register` dan `login`, route `auth` tidak membatasi role tapi login/register tetap ada di endpoint publik, sementara operasi CRUD lain yang mengubah data memang memerlukan JWT role `Admin`.

## Kesimpulan aksesibilitas endpoint

- Public: `GET` semua resource, `POST /auth/register`, `POST /auth/login`.
- Protected: `POST/PUT/DELETE` di `Profile`, `Contact`, `Project`, `TechStack`, `Skill`, dan `Achievement`; semua memerlukan JWT Bearer dengan role `Admin`.
- Poin penting untuk frontend: gunakan `Authorization: Bearer <token>` hanya untuk endpoint admin, dan pastikan parsing data untuk field `Photo_Url`, `Icon_Url`, `Photos`, `TechStack_Ids`, serta `password` di login dilakukan dengan hati-hati.
