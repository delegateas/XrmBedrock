USE DataMigration
GO 

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmProdJobTbl_After_Insert_Trg]') AND type in (N'TR'))
DROP TRIGGER [dbo].[DmProdJobTbl_After_Insert_Trg]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmProdJobTbl_After_Update_Trg]') AND type in (N'TR'))
DROP TRIGGER [dbo].[DmProdJobTbl_After_Update_Trg]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DmProdJobTbl]') AND type in (N'U'))
DROP TABLE [dbo].[DmProdJobTbl]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DmProdJobTbl](
	[DmId] [int] NOT NULL Identity(1,1),
	[DmJobType] [nvarchar](100) NOT NULL,
	[DmPriority] [int] NULL,
	[DmCreatedOn] [datetime] NULL,
	[DmModifiedOn] [datetime] NULL,
	[DmStatus] [nchar](30) NOT NULL,
	[DmMessage] [ntext] NULL,
	[DmErrorsOccurred] [bit] NULL,
	[DmRunnerScope] [nvarchar](50) NULL,
 CONSTRAINT [PK_DmProdJobTbl] PRIMARY KEY CLUSTERED 
(
	[DmId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TRIGGER DmProdJobTbl_After_Insert_Trg
    ON dbo.DmProdJobTbl
    FOR INSERT
    AS
    BEGIN
        SET NOCOUNT ON
        
        UPDATE dbo.DmProdJobTbl
            SET DmCreatedOn = GETUTCDATE(),
                DmModifiedOn = GETUTCDATE()
        FROM dbo.DmProdJobTbl INNER JOIN inserted 
            ON dbo.DmProdJobTbl.DmId = inserted.DmId;
    END
GO

CREATE TRIGGER DmProdJobTbl_After_Update_Trg
    ON dbo.DmProdJobTbl
    FOR UPDATE
    AS
    BEGIN
        SET NOCOUNT ON
        
        IF ( (SELECT trigger_nestlevel() ) > 1 )
            RETURN
            
        UPDATE dbo.DmProdJobTbl
            SET DmCreatedOn = deleted.DmCreatedOn,
                DmModifiedOn = GETUTCDATE()
        FROM dbo.DmProdJobTbl 
        INNER JOIN deleted
            ON dbo.DmProdJobTbl.DmId = deleted.DmId;
        
    END
GO