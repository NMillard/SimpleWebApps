IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220513204501_Initial')
BEGIN
    CREATE TABLE [Schedules] (
        [Id] uniqueidentifier NOT NULL,
        [TaskName] nvarchar(150) NOT NULL,
        [StartDate] DATE NOT NULL,
        [EndDate] DATE NULL,
        [TimeOfDay] TIME NOT NULL,
        [IntervalInDays] int NOT NULL,
        CONSTRAINT [PK_Schedules] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220513204501_Initial')
BEGIN
    CREATE TABLE [ScheduleRuns] (
        [TaskName] nvarchar(150) NOT NULL,
        [CorrelationId] uniqueidentifier NOT NULL,
        [State] nvarchar(450) NOT NULL,
        [Time] datetimeoffset NOT NULL,
        [ScheduleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScheduleRuns] PRIMARY KEY ([TaskName], [CorrelationId], [State]),
        CONSTRAINT [FK_ScheduleRuns_Schedules_ScheduleId] FOREIGN KEY ([ScheduleId]) REFERENCES [Schedules] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220513204501_Initial')
BEGIN
    CREATE INDEX [IX_ScheduleRuns_ScheduleId] ON [ScheduleRuns] ([ScheduleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220513204501_Initial')
BEGIN
    CREATE UNIQUE INDEX [IX_Schedules_TaskName] ON [Schedules] ([TaskName]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220513204501_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220513204501_Initial', N'6.0.5');
END;
GO

COMMIT;
GO

