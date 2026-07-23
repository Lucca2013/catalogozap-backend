using CatalogoZap.Repositories;
using System.Data;
using Dapper;
using CatalogoZap.Models;

namespace CatalogoZap.Repositories;

public sealed class ProfilesRepository(
        IDbConnection conn
    )
{
    public async Task<ProfileModel?> GetProfileById(Guid userId)
    {
        var query = @"
            SELECT 
                id AS Id,
                username AS Username,
                bio AS Bio,
                phone AS Phone,
                logo_url AS LogoUrl,
                created_at AS CreatedAt,
                email AS Email,
                premium AS Premium,
                password AS Password
            FROM profiles 
            WHERE id = @userId
        ";

        var profile = await conn.QuerySingleOrDefaultAsync<ProfileModel>(query, new { userId });

        return profile;
    }

    public async Task<ProfileModel?> PublicGetProfileById(Guid userId)
    {
        var query = @"
            SELECT 
                id AS Id,
                username AS Username,
                bio AS Bio,
                phone AS Phone,
                logo_url AS LogoUrl,
                created_at AS CreatedAt,
                email AS Email,
                premium AS Premium
            FROM profiles 
            WHERE id = @userId
        ";

        var profile = await conn.QuerySingleOrDefaultAsync<ProfileModel>(query, new { userId });

        return profile;
    }

    public async Task<LoginModel?> GetProfileByEmail(string Email)
    {
        var query = @"
            SELECT 
                username AS Username,
                password AS Password,
                id AS Id
            FROM profiles
            WHERE email = @email";

        return await conn.QuerySingleOrDefaultAsync<LoginModel>(query, new
        {
            email = Email
        });
    }

    public async Task InsertUser(RegisterModel register)
    {
        var query = @"
            INSERT INTO profiles(username, email, password) VALUES(@username, @email, @password)
        ";

        await conn.ExecuteAsync(query, new
        {
            username = register.Username,
            email = register.Email,
            password = register.HashPassword
        });
    }

    public async Task ModifyProfile(Guid userId, ProfileModel newdata)
    {
        var query = @"
            UPDATE profiles
            SET
                username = @Username,
                bio = @Bio,
                phone = @Phone,
                logo_url = @LogoUrl,
                email = @Email,
                password = @Password
            where id = @Id
        ";
        await conn.ExecuteAsync(query, newdata);
    }
}
