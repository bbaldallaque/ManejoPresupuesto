-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Transacciones_Insertar]
  @UsuarioId int,
  @fechaTransaccion date,
  @Monto decimal(18,2),
  @CategoriaId int,
  @CuentaId int,
  @nota nvarchar(1000) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Insert into Transacciones
	(UsuarioId,
	FechaTransaccion,
	Monto,
	CategoriaId,
	CuentaId,
	Nota)
	values	
	(@UsuarioId,
	@fechaTransaccion,
	ABS (@Monto),
	@CategoriaId,
	@CuentaId,
	@nota)

	UPDATE Cuentas
	SET Balance += @Monto
	WHERE Id = @CuentaId;

	SELECT SCOPE_IDENTITY();
END
