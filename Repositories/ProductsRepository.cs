using CatalogoZap.Repositories;
using System.Data;
using Dapper;
using CatalogoZap.Models;

namespace CatalogoZap.Repositories;

public sealed class ProductsRepository(
        IDbConnection conn
    )
{
    public async Task<int> GetProductsAmountByUserId(Guid userId)
    {
        var query = @"
            SELECT COUNT(*)
            FROM products p
            INNER JOIN profiles u ON p.user_id = u.id
            WHERE user_id = @userId
        ";

        return await conn.QuerySingleAsync<int>(query, new { userId });
    }

    public async Task CreateProduct(ProductModel data)
    {
        var query = @"
            INSERT INTO products
                (user_id, name, price_cents, photo_url, store_id, avaliable)
            VALUES
                (@UserId, @Name, @PriceCents, @PhotoUrl, @StoreId, @Avaliable)
        ";

        await conn.ExecuteScalarAsync(query, new
        {
            data.UserId,
            data.Name,
            data.PriceCents,
            data.PhotoUrl,
            data.StoreId,
            data.Avaliable
        });
    }

    public async Task<List<ProductModel>> GetProducts(Guid storeId)
    {
        var query = @"
        SELECT 
            id AS Id,
            user_id AS UserId,
            name AS Name,
            price_cents AS PriceCents,
            photo_url AS PhotoUrl,
            store_id AS StoreId,
            avaliable AS Avaliable,
            created_at AS Created_at
        FROM products 
        WHERE store_id = @store_id AND avaliable = TRUE";

        var products = await conn.QueryAsync<ProductModel>(query, new
        {
            store_Id = storeId
        });

        return products.ToList();
    }

    public async Task<ProductModel?> GetProductById(Guid Id, Guid StoreId, Guid UserId)
    {
        var query = @"
        SELECT 
            id AS Id,
            user_id AS UserId,
            name AS Name,
            price_cents AS PriceCents,
            photo_url AS PhotoUrl,
            store_id AS StoreId,
            avaliable AS Avaliable
        FROM products 
        WHERE id = @id AND store_id = @store_id AND user_id = @user_id";

        var products = await conn.QuerySingleOrDefaultAsync<ProductModel>(query, new
        {
            id = Id,
            store_id = StoreId,
            user_id = UserId
        });

        return products;
    }

    public async Task<List<ProductModel>> GetProductsAdmin(Guid storeId, Guid? UserId)
    {
        var query = @"
        SELECT 
            id AS Id,
            user_id AS UserId,
            name AS Name,
            price_cents AS PriceCents,
            photo_url AS PhotoUrl,
            store_id AS StoreId,
            avaliable AS Avaliable
        FROM products 
        WHERE store_id = @store_id AND user_id = @user_id";

        var products = await conn.QueryAsync<ProductModel>(query, new
        {
            store_Id = storeId,
            user_id = UserId
        });

        return products.ToList();
    }

    public async Task ModifyProducts(ProductModel product)
    {
        var query = @"
            UPDATE products
            SET 
                name = @Name,
                price_cents = @PriceCents,
                photo_url = @PhotoUrl,
                avaliable = @Avaliable
            WHERE store_id = @StoreId
            AND user_id = @UserId
        ";
        
        await conn.QueryAsync(query, product);
    }

    public async Task DeleteProduct(Guid Id, Guid StoreId, Guid UserID)
    {
        var query = @"
            DELETE FROM products WHERE id = @Id AND store_id = @storeId AND user_id = @userID
        ";

        await conn.QueryAsync(query, new
        {
            userID = UserID,
            storeId = StoreId,
            Id
        });
    }
}
