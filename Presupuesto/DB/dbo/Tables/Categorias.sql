CREATE TABLE [dbo].[Categorias] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]          NVARCHAR (50) NOT NULL,
    [TipoOperacionId] INT           NOT NULL,
    [UsuarioId]       INT           NOT NULL,
    CONSTRAINT [PK_Categorias] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Categorias_TipoOperaciones] FOREIGN KEY ([TipoOperacionId]) REFERENCES [dbo].[TipoOperaciones] ([Id]),
    CONSTRAINT [FK_Categorias_Usuarios] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuarios] ([Id])
);

