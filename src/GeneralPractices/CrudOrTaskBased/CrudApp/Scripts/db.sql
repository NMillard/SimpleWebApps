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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210810112853_Initial')
BEGIN
    CREATE TABLE [Authors] (
        [Id] uniqueidentifier NOT NULL,
        [PenNames] nvarchar(max) NOT NULL,
        [RealName] nvarchar(max) NOT NULL,
        [Bio] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Authors] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210810112853_Initial')
BEGIN
    CREATE TABLE [ChatChannels] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_ChatChannels] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210810112853_Initial')
BEGIN
    CREATE TABLE [ChatMessage] (
        [Id] uniqueidentifier NOT NULL,
        [Content] nvarchar(1000) NOT NULL,
        [ChatChannelId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ChatMessage] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChatMessage_ChatChannels_ChatChannelId] FOREIGN KEY ([ChatChannelId]) REFERENCES [ChatChannels] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210810112853_Initial')
BEGIN
    CREATE INDEX [IX_ChatMessage_ChatChannelId] ON [ChatMessage] ([ChatChannelId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210810112853_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210810112853_Initial', N'5.0.8');
END;
GO

COMMIT;
GO

