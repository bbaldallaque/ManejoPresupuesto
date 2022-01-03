CREATE TABLE [dbo].[TipoOperaciones] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Descripcion] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_TipoOperaciones] PRIMARY KEY CLUSTERED ([Id] ASC)
);

