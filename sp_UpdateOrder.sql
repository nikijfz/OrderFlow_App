CREATE OR ALTER PROCEDURE dbo.Sp_UpdateOrder
    @JsonData NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @GuidKey UNIQUEIDENTIFIER = JSON_VALUE(@JsonData, '$.GuidKey');
        DECLARE @BuyerId UNIQUEIDENTIFIER = JSON_VALUE(@JsonData, '$.BuyerId'); 
        DECLARE @TotalAmount DECIMAL(18,2) = JSON_VALUE(@JsonData, '$.TotalAmount');
        DECLARE @OrderDate DATETIME2 = JSON_VALUE(@JsonData, '$.OrderDate');
        DECLARE @HeaderId UNIQUEIDENTIFIER;

        SELECT @HeaderId = Id 
        FROM Sales.OrderHeaders 
        WHERE GuidKey = @GuidKey AND IsDeleted = 0;

        IF @HeaderId IS NULL
        BEGIN
            RAISERROR('Order not found or already deleted.', 16, 1);
            RETURN;
        END

        UPDATE Sales.OrderHeaders
        SET BuyerId = @BuyerId,
            TotalAmount = @TotalAmount,
            OrderDate = ISNULL(@OrderDate, OrderDate)
        WHERE Id = @HeaderId;

        MERGE Sales.OrderDetails AS target
        USING (
            SELECT 
                d.GuidKey,
                d.ProductId,
                d.UnitPrice,
                d.Amount
            FROM OPENJSON(@JsonData, '$.Details') 
            WITH (
                GuidKey UNIQUEIDENTIFIER '$.GuidKey', 
                ProductId UNIQUEIDENTIFIER '$.ProductId', 
                UnitPrice DECIMAL(18,2) '$.UnitPrice', 
                Amount DECIMAL(18,2) '$.Amount'
            ) AS d
        ) AS source
        ON target.GuidKey = source.GuidKey AND target.OrderHeaderId = @HeaderId

        WHEN MATCHED THEN
            UPDATE SET 
                target.UnitPrice = source.UnitPrice, 
                target.Amount = source.Amount,
                target.IsDeleted = 0 

        WHEN NOT MATCHED THEN
            INSERT (Id, GuidKey, OrderHeaderId, ProductId, UnitPrice, Amount, IsDeleted)
            VALUES (NEWID(), source.GuidKey, @HeaderId, source.ProductId, source.UnitPrice, source.Amount, 0)

        WHEN NOT MATCHED BY SOURCE AND target.OrderHeaderId = @HeaderId THEN
            DELETE;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO