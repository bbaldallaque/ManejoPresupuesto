CREATE TABLE [dbo].[TipoCuentas] (
    [Id]        INT        IDENTITY (1, 1) NOT NULL,
    [Nombre]    NCHAR (50) NOT NULL,
    [UsuarioId] INT        NOT NULL,
    [Orden]     INT        NOT NULL,
    CONSTRAINT [PK_TipoCuentas] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TipoCuentas_Usuarios] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuarios] ([Id])
);

