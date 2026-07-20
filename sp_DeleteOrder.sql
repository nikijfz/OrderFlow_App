CREATE OR ALTER PROCEDURE dbo.Sp_DeleteOrder
    @JsonData NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @Guid UNIQUEIDENTIFIER = CAST(JSON_VALUE(@JsonData, '$.GuidKey') AS UNIQUEIDENTIFIER);

        UPDATE od
        SET od.IsDeleted = 1
        FROM Sales.OrderDetails od
        INNER JOIN Sales.OrderHeaders oh ON od.OrderHeaderId = oh.Id
        WHERE oh.GuidKey = @Guid;

        UPDATE Sales.OrderHeaders
        SET IsDeleted = 1
        WHERE GuidKey = @Guid;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO
