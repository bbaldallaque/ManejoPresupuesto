-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TiposCuentaInsertar
	
	@Nombre nvarchar(50),
	@usuarioId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
	DECLARE @Orden int;
	SELECT @Orden = COALESCE(MAX(Orden), 0) + 1
	FROM TipoCuentas
	WHERE UsuarioId = @usuarioId

	INSERT INTO  TipoCuentas(Nombre, UsuarioId, Orden)
	VALUES (@Nombre, @usuarioId, @Orden);

	SELECT SCOPE_IDENTITY();

END
