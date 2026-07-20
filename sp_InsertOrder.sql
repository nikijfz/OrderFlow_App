CREATE OR ALTER PROCEDURE dbo.Sp_InsertOrder
    @JsonData NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @HeaderId UNIQUEIDENTIFIER = NEWID();

        -- Insert Order Header
        INSERT INTO Sales.OrderHeaders (Id, GuidKey, SellerId, BuyerId, OrderDate, TotalAmount, IsDeleted)
        SELECT @HeaderId, h.GuidKey, h.SellerId, h.BuyerId, h.OrderDate, h.TotalAmount, 0
        FROM OPENJSON(@JsonData) WITH (
            GuidKey UNIQUEIDENTIFIER '$.GuidKey', 
            SellerId UNIQUEIDENTIFIER '$.SellerId', 
            BuyerId UNIQUEIDENTIFIER '$.BuyerId', 
            OrderDate DATETIME2 '$.OrderDate', 
            TotalAmount DECIMAL(18,2) '$.TotalAmount'
        ) AS h;

        -- Insert Order Details
        INSERT INTO Sales.OrderDetails (Id, GuidKey, OrderHeaderId, ProductId, UnitPrice, Amount, IsDeleted)
        SELECT 
            NEWID(), 
            d.GuidKey, 
            @HeaderId, 
            d.ProductId, 
            d.UnitPrice, 
            d.Amount, 
            0
        FROM OPENJSON(@JsonData, '$.Details') 
        WITH (
            GuidKey UNIQUEIDENTIFIER '$.GuidKey', 
            ProductId UNIQUEIDENTIFIER '$.ProductId', 
            UnitPrice DECIMAL(18,2) '$.UnitPrice', 
            Amount DECIMAL(18,2) '$.Amount'
        ) AS d;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
