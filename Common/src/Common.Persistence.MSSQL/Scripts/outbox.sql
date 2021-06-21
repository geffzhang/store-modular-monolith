CREATE TABLE [dbo].OutboxMessage
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [OccurredOn] DATETIME2 NOT NULL,
    [Type] VARCHAR(255) NOT NULL,
    [Name] VARCHAR(255) NOT NULL,
    [Payload] VARCHAR(MAX) NOT NULL,
    [SentAt] DATETIME2 NULL,
    [ModuleName] Nvarchar(100) Not NULL,
    CONSTRAINT [PK_OutboxMessage_Id] PRIMARY KEY ([Id] ASC)
)
Go