

IF DB_ID('TaskManagementDb') IS NULL
BEGIN
    CREATE DATABASE TaskManagementDb;
END
GO

USE TaskManagementDb;
GO

CREATE TABLE Users (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    FullName      NVARCHAR(100)  NOT NULL,
    Email         NVARCHAR(150)  NOT NULL,
    PasswordHash  NVARCHAR(MAX)  NOT NULL,
    Role          INT            NOT NULL DEFAULT 2,
    CreatedAt     DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    TeamId        INT            NULL,
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

CREATE TABLE Teams (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(100)  NOT NULL,
    Description   NVARCHAR(MAX)  NULL,
    ManagerId     INT            NOT NULL,
    CreatedAt     DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Teams_Manager FOREIGN KEY (ManagerId) REFERENCES Users(Id)
);
GO

ALTER TABLE Users
    ADD CONSTRAINT FK_Users_Team FOREIGN KEY (TeamId) REFERENCES Teams(Id) ON DELETE SET NULL;
GO

CREATE TABLE TaskItems (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Title         NVARCHAR(200)  NOT NULL,
    Description   NVARCHAR(MAX)  NULL,
    Status        INT            NOT NULL DEFAULT 0,
    Priority      INT            NOT NULL DEFAULT 1,
    Deadline      DATETIME2      NULL,
    TeamId        INT            NOT NULL,
    AssignedToId  INT            NOT NULL,
    CreatedById   INT            NOT NULL,
    CreatedAt     DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt     DATETIME2      NULL,
    CONSTRAINT FK_TaskItems_Team       FOREIGN KEY (TeamId)       REFERENCES Teams(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TaskItems_AssignedTo FOREIGN KEY (AssignedToId) REFERENCES Users(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_TaskItems_CreatedBy  FOREIGN KEY (CreatedById)  REFERENCES Users(Id) ON DELETE NO ACTION
);
GO

CREATE TABLE TaskComments (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    TaskItemId    INT            NOT NULL,
    UserId        INT            NOT NULL,
    Content       NVARCHAR(MAX)  NOT NULL,
    CreatedAt     DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_TaskComments_TaskItem FOREIGN KEY (TaskItemId) REFERENCES TaskItems(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TaskComments_User     FOREIGN KEY (UserId)     REFERENCES Users(Id)     ON DELETE NO ACTION
);
GO

CREATE TABLE Notifications (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    UserId        INT            NOT NULL,
    Type          INT            NOT NULL,
    Message       NVARCHAR(MAX)  NOT NULL,
    TaskItemId    INT            NULL,
    IsRead        BIT            NOT NULL DEFAULT 0,
    CreatedAt     DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Notifications_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

PRINT 'Schema created successfully.';
