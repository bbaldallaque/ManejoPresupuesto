﻿CREATE TABLE [dbo].[Cuentas] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]       NVARCHAR (50)   NOT NULL,
    [TipoCuentaId] INT             NOT NULL,
    [Balance]      DECIMAL (18, 2) NOT NULL,
    [Descripcion]  NCHAR (1000)    NULL,
    CONSTRAINT [PK_Cuentas] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Cuentas_TipoCuentas] FOREIGN KEY ([TipoCuentaId]) REFERENCES [dbo].[TipoCuentas] ([Id])
);

