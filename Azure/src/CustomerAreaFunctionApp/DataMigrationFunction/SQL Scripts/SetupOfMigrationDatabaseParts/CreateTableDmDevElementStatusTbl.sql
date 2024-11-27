USE DataMigration
GO 

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmDevElementStatusTbl_After_Insert_Trg]') AND type in (N'TR'))
DROP TRIGGER [dbo].[DmDevElementStatusTbl_After_Insert_Trg]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmDevElementStatusTbl_After_Update_Trg]') AND type in (N'TR'))
DROP TRIGGER [dbo].[DmDevElementStatusTbl_After_Update_Trg]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmDevElementStatusTbl]') AND type in (N'U'))
DROP TABLE [dbo].[DmDevElementStatusTbl]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DmDevElementStatusTbl](
	[DmId] [int] NOT NULL Identity(1,1),
	[DmJobType] [nvarchar](100) NOT NULL,
	[DmElementId] [nvarchar](100) NOT NULL,
	[DmCreatedOn] [datetime] NULL,
	[DmModifiedOn] [datetime] NULL,
	[DmStatus] [nchar](10) NULL,
	[DmMessage] [ntext] NULL,
 CONSTRAINT [PK_DmDevElementStatusTbl] PRIMARY KEY CLUSTERED 
(
	[DmId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TRIGGER DmDevElementStatusTbl_After_Insert_Trg
    ON dbo.DmDevElementStatusTbl
    FOR INSERT
    AS
    BEGIN
        SET NOCOUNT ON
        
        UPDATE dbo.DmDevElementStatusTbl
            SET DmCreatedOn = GETUTCDATE(),
                DmModifiedOn = GETUTCDATE()
        FROM dbo.DmDevElementStatusTbl INNER JOIN inserted 
            ON dbo.DmDevElementStatusTbl.DmId = inserted.DmId;
    END
GO

CREATE TRIGGER DmDevElementStatusTbl_After_Update_Trg
    ON dbo.DmDevElementStatusTbl
    FOR UPDATE
    AS
    BEGIN
        SET NOCOUNT ON
        
        IF ( (SELECT trigger_nestlevel() ) > 1 )
            RETURN
            
        UPDATE dbo.DmDevElementStatusTbl
            SET DmCreatedOn = deleted.DmCreatedOn,
                DmModifiedOn = GETUTCDATE()
        FROM dbo.DmDevElementStatusTbl 
        INNER JOIN deleted
            ON dbo.DmDevElementStatusTbl.DmId = deleted.DmId;
        
    END
GO