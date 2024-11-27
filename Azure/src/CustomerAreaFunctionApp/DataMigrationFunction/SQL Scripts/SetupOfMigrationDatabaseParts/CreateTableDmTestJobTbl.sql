USE DataMigration
GO 

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmTestJobTbl_After_Insert_Trg]') AND type in (N'TR'))
DROP TRIGGER [dbo].[DmTestJobTbl_After_Insert_Trg]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmTestJobTbl_After_Update_Trg]') AND type in (N'TR'))
DROP TRIGGER [dbo].[DmTestJobTbl_After_Update_Trg]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmTestJobTbl]') AND type in (N'U'))
DROP TABLE [dbo].[DmTestJobTbl]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DmTestJobTbl](
	[DmId] [int] NOT NULL Identity(1,1),
	[DmJobType] [nvarchar](100) NOT NULL,
	[DmPriority] [int] NULL,
	[DmCreatedOn] [datetime] NULL,
	[DmModifiedOn] [datetime] NULL,
	[DmStatus] [nchar](30) NOT NULL,
	[DmMessage] [ntext] NULL,
	[DmErrorsOccurred] [bit] NULL,
	[DmRunnerScope] [nvarchar](50) NULL,
 CONSTRAINT [PK_DmTestJobTbl] PRIMARY KEY CLUSTERED 
(
	[DmId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TRIGGER DmTestJobTbl_After_Insert_Trg
    ON dbo.DmTestJobTbl
    FOR INSERT
    AS
    BEGIN
        SET NOCOUNT ON
        
        UPDATE dbo.DmTestJobTbl
            SET DmCreatedOn = GETUTCDATE(),
                DmModifiedOn = GETUTCDATE()
        FROM dbo.DmTestJobTbl INNER JOIN inserted 
            ON dbo.DmTestJobTbl.DmId = inserted.DmId;
    END
GO

CREATE TRIGGER DmTestJobTbl_After_Update_Trg
    ON dbo.DmTestJobTbl
    FOR UPDATE
    AS
    BEGIN
        SET NOCOUNT ON
        
        IF ( (SELECT trigger_nestlevel() ) > 1 )
            RETURN
            
        UPDATE dbo.DmTestJobTbl
            SET DmCreatedOn = deleted.DmCreatedOn,
                DmModifiedOn = GETUTCDATE()
        FROM dbo.DmTestJobTbl 
        INNER JOIN deleted
            ON dbo.DmTestJobTbl.DmId = deleted.DmId;
        
    END
GO