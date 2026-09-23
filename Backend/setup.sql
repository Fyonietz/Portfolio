-- Full database schema for Portfolio backend (SQLite)

PRAGMA foreign_keys = ON;

-- Users (admin)
CREATE TABLE IF NOT EXISTS "User" (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Username TEXT NOT NULL,
  Email TEXT NOT NULL UNIQUE,
  Password TEXT NOT NULL,
  ImageUrl TEXT
);

-- Single profile row for public site info
CREATE TABLE IF NOT EXISTS Profile (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Name TEXT NOT NULL,
  Role_Title TEXT NOT NULL,
  Photo_Url TEXT,
  Status TEXT,
  Bio TEXT,
  Updated_At TEXT
);

-- Contacts (no FK to Profile; single profile assumed)
CREATE TABLE IF NOT EXISTS Contact (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Platform TEXT NOT NULL,
  Value TEXT NOT NULL,
  Sort_Order INTEGER NOT NULL DEFAULT 0
);

-- Projects (card list -> detail pages)
CREATE TABLE IF NOT EXISTS Project (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Title TEXT NOT NULL,
  Slug TEXT NOT NULL UNIQUE,
  Short_Description TEXT,
  Full_Description TEXT,
  Photos TEXT, -- JSON array of photo filenames/urls
  Tech_Stack TEXT, -- comma-separated or JSON
  Repo_Url TEXT,
  Demo_Url TEXT,
  Sort_Order INTEGER NOT NULL DEFAULT 0,
  Published INTEGER NOT NULL DEFAULT 0,
  Created_At TEXT,
  Updated_At TEXT
);

-- Achievements
CREATE TABLE IF NOT EXISTS Achievement (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Title TEXT NOT NULL,
  Description TEXT,
  Achieved_At TEXT,
  Sort_Order INTEGER NOT NULL DEFAULT 0
);


