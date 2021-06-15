--CHANGE NAMES ACCORDINGLY

CREATE TABLE [Playground].[Outbox]
(
    [Id] [bigint] NOT NULL IDENTITY(1, 1),
    [EventId] [nvarchar] (200) NOT NULL,
    [Body] [ntext] NOT NULL,
    [Exchange] [nvarchar] (1000)  NOT NULL,
    [StatusId] [tinyint] NOT NULL,
    [BrokerTypeId] [BIGINT] NULL,
    [PickedDate] [datetime2] NULL,
    [DeliveredDate] [datetime2] NULL,
    [DeliveryCount] [tinyint] NULL,
    [CreatedBy] [nvarchar] (200)  NULL,
    [CreatedDate] [datetime2] (3) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [Playground].[Outbox] ADD CONSTRAINT [PK_Playground_Outbox] PRIMARY KEY CLUSTERED ([Id]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [NCIX_Playground_Outbox_StatusId_PickedDate] ON [Playground].[Outbox] ([StatusId], [PickedDate]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [NCIX_Playground_Outbox_Exchange_EventId] ON [Playground].[Outbox] ([Exchange], [EventId]) ON [PRIMARY]
GO