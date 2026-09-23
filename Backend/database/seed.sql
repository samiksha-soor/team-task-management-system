

USE TaskManagementDb;
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@example.com')
BEGIN

    INSERT INTO Users (FullName, Email, PasswordHash, Role, CreatedAt)
    VALUES
        ('Alice Admin',   'admin@example.com',   '$2b$11$fphEEFxfgJ6pyo9BO.DIv.46r.zAtw.CHKQMdaIGQ3OZzmmSeQJfa', 0, SYSUTCDATETIME()),
        ('Mo Manager',    'manager@example.com', '$2b$11$cHoF/DHuIq9BWqsxGaQ16.saVlsk1Zc1YwCVDFNGNsHIZ6d4POzGm', 1, SYSUTCDATETIME()),
        ('Uma User',      'user1@example.com',   '$2b$11$trrDZ9ngH2fcStaaGF5Px.k5MkJNfsgQkLcoQ0Rp.XK2tfN/8s.qq', 2, SYSUTCDATETIME()),
        ('Zed User',      'user2@example.com',   '$2b$11$trrDZ9ngH2fcStaaGF5Px.k5MkJNfsgQkLcoQ0Rp.XK2tfN/8s.qq', 2, SYSUTCDATETIME());

    DECLARE @ManagerId INT = (SELECT Id FROM Users WHERE Email = 'manager@example.com');
    DECLARE @User1Id   INT = (SELECT Id FROM Users WHERE Email = 'user1@example.com');
    DECLARE @User2Id   INT = (SELECT Id FROM Users WHERE Email = 'user2@example.com');
    DECLARE @AdminId   INT = (SELECT Id FROM Users WHERE Email = 'admin@example.com');

    INSERT INTO Teams (Name, Description, ManagerId, CreatedAt)
    VALUES ('Engineering', 'Core product engineering team', @ManagerId, SYSUTCDATETIME());

    DECLARE @TeamId INT = SCOPE_IDENTITY();

    UPDATE Users SET TeamId = @TeamId WHERE Id IN (@User1Id, @User2Id);

    INSERT INTO TaskItems (Title, Description, Status, Priority, Deadline, TeamId, AssignedToId, CreatedById, CreatedAt)
    VALUES
        ('Design database schema', 'Draft the ERD for the task system', 0, 2, DATEADD(day, 7, SYSUTCDATETIME()), @TeamId, @User1Id, @ManagerId, SYSUTCDATETIME()),
        ('Set up CI pipeline',     'Configure GitHub Actions for build + test', 1, 1, DATEADD(day, 3, SYSUTCDATETIME()), @TeamId, @User2Id, @ManagerId, SYSUTCDATETIME());

    PRINT 'Seed data inserted: 1 admin, 1 manager, 2 users, 1 team, 2 tasks.';
END
ELSE
BEGIN
    PRINT 'Seed data already present - skipping.';
END
GO
