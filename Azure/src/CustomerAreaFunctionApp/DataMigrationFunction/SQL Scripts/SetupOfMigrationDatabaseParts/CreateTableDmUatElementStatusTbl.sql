USE DataMigration
GO 

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmUatElementStatusTbl_After_Insert_Trg]') AND type in (N'TR'))
DROP TRIGGER [dbo].[DmUatElementStatusTbl_After_Insert_Trg]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmUatElementStatusTbl_After_Update_Trg]') AND type in (N'TR'))
DROP TRIGGER [dbo].[DmUatElementStatusTbl_After_Update_Trg]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmUatElementStatusTbl]') AND type in (N'U'))
DROP TABLE [dbo].[DmUatElementStatusTbl]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DmUatElementStatusTbl](
	[DmId] [int] NOT NULL Identity(1,1),
	[DmJobType] [nvarchar](100) NOT NULL,
	[DmElementId] [nvarchar](100) NOT NULL,
	[DmCreatedOn] [datetime] NULL,
	[DmModifiedOn] [datetime] NULL,
	[DmStatus] [nchar](10) NULL,
	[DmMessage] [ntext] NULL,
 CONSTRAINT [PK_DmUatElementStatusTbl] PRIMARY KEY CLUSTERED 
(
	[DmId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TRIGGER DmUatElementStatusTbl_After_Insert_Trg
    ON dbo.DmUatElementStatusTbl
    FOR INSERT
    AS
    BEGIN
        SET NOCOUNT ON
        
        UPDATE dbo.DmUatElementStatusTbl
            SET DmCreatedOn = GETUTCDATE(),
                DmModifiedOn = GETUTCDATE()
        FROM dbo.DmUatElementStatusTbl INNER JOIN inserted 
            ON dbo.DmUatElementStatusTbl.DmId = inserted.DmId;
    END
GO

CREATE TRIGGER DmUatElementStatusTbl_After_Update_Trg
    ON dbo.DmUatElementStatusTbl
    FOR UPDATE
    AS
    BEGIN
        SET NOCOUNT ON
        
        IF ( (SELECT trigger_nestlevel() ) > 1 )
            RETURN
            
        UPDATE dbo.DmUatElementStatusTbl
            SET DmCreatedOn = deleted.DmCreatedOn,
                DmModifiedOn = GETUTCDATE()
        FROM dbo.DmUatElementStatusTbl 
        INNER JOIN deleted
            ON dbo.DmUatElementStatusTbl.DmId = deleted.DmId;
        
    END
GO