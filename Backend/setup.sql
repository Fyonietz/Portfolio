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
  Description TEXT,
  Photo_Url TEXT,
  Status TEXT,
  Bio TEXT,
  Updated_At TEXT
);

-- Contacts
CREATE TABLE IF NOT EXISTS Contact (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Platform TEXT NOT NULL,
  Value TEXT NOT NULL,
  Sort_Order INTEGER NOT NULL DEFAULT 0
);

-- Tech Stack (master list, reusable across projects)
CREATE TABLE IF NOT EXISTS TechStack (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Name TEXT NOT NULL UNIQUE,
  Icon_Url TEXT,
  Sort_Order INTEGER NOT NULL DEFAULT 0
);

-- Projects
CREATE TABLE IF NOT EXISTS Project (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Title TEXT NOT NULL,
  Slug TEXT NOT NULL UNIQUE,
  Short_Description TEXT,
  Full_Description TEXT,
  Photos TEXT,
  Repo_Url TEXT,
  Demo_Url TEXT,
  Sort_Order INTEGER NOT NULL DEFAULT 0,
  Published INTEGER NOT NULL DEFAULT 0,
  Created_At TEXT,
  Updated_At TEXT
);

-- Project <-> TechStack (many to many)
CREATE TABLE IF NOT EXISTS Project_TechStack (
  Project_Id INTEGER NOT NULL,
  TechStack_Id INTEGER NOT NULL,
  PRIMARY KEY (Project_Id, TechStack_Id),
  FOREIGN KEY (Project_Id) REFERENCES Project(Id) ON DELETE CASCADE,
  FOREIGN KEY (TechStack_Id) REFERENCES TechStack(Id) ON DELETE CASCADE
);

-- Achievements
-- Achievements
CREATE TABLE IF NOT EXISTS Achievement (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Title TEXT NOT NULL,
  Slug TEXT NOT NULL UNIQUE,
  Description TEXT,
  Photo_Url TEXT,
  Achieved_At TEXT,
  Sort_Order INTEGER NOT NULL DEFAULT 0
);

-- Skills
CREATE TABLE IF NOT EXISTS Skill (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Name TEXT NOT NULL,
  Category TEXT,
  Description TEXT,
  Icon_Url TEXT,
  Level INTEGER,
  Sort_Order INTEGER NOT NULL DEFAULT 0,
  Published INTEGER NOT NULL DEFAULT 1,
  Created_At TEXT,
  Updated_At TEXT
);
