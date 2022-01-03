-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE CrearDatosUsuariosNuevo
	-- Add the parameters for the stored procedure here
	@usuarioId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @Efectivo nvarchar(50) = 'Efectivo';
	DECLARE @CuentasDeBanco nvarchar(50) = 'Cuentas de Banco';
	DECLARE @Tarjetas nvarchar(50) = 'Tarjetas';

	INSERT INTO TipoCuentas(Nombre, UsuarioId, Orden)
	VALUES (@Efectivo, @usuarioId, 1),
	(@CuentasDeBanco, @usuarioId, 2),
	(@Tarjetas, @usuarioId, 3);
   
   INSERT INTO Cuentas(Nombre, Balance, TipoCuentaId)
   SELECT Nombre, 0, Id
   FROM TipoCuentas
   WHERE UsuarioId = @usuarioId;

   INSERT INTO Categorias(Nombre, TipoOperacionId, UsuarioId)
   VALUES
   ('Prestamo', 2, @usuarioId),
   ('Salario', 1, @usuarioId),
   ('Prestamo del Banco', 2, @usuarioId),
   ('Extra', 1, @usuarioId)


END
