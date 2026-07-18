using CatalogoZap.DTOs;
using CatalogoZap.Infrastructure.CloudinaryService;
using CatalogoZap.Infrastructure.Exceptions;
using CatalogoZap.Infrastructure.JWT;
using CatalogoZap.Infrastructure.SendGrid;
using CatalogoZap.Models;
using CatalogoZap.Repositories.Interfaces;
using CatalogoZap.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;

namespace CatalogoZap.Services;

public class ProfilesService : IProfilesService
{
    private readonly IProductsRepository _productsRepository;
    private readonly IProfilesRepository _profilesRepository;
    private readonly ITokenService _tokenService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ISendGridService _sendGridService;
    private readonly IConfiguration _config;

    public ProfilesService(
        IProductsRepository productsRepository,
        IProfilesRepository profilesRepository,
        ITokenService tokenService,
        ICloudinaryService cloudinaryService,
        ISendGridService sendGridService,
        IConfiguration config)
    {
        _productsRepository = productsRepository;
        _profilesRepository = profilesRepository;
        _tokenService = tokenService;
        _cloudinaryService = cloudinaryService;
        _sendGridService = sendGridService;
        _config = config;
    }

    public async Task<bool> HasReachedFreeTierLimit(Guid userId)
    {
        var productsAmount = await _productsRepository.GetProductsAmountByUserId(userId);
        return productsAmount >= 10;
    }

    public async Task<string> Login(LoginDTO dto)
    {
        var dbData = await _profilesRepository.GetProfileByEmail(dto.Email)
            ?? throw new UnauthorizedException("User doesnt exist");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, dbData.Password))
            throw new UnauthorizedException("Incorrect Password");

        return _tokenService.GenerateToken(dbData.Id, 24);
    }

    public async Task Register(RegisterDTO dto)
    {
        if (await _profilesRepository.GetProfileByEmail(dto.Email) != null)
            throw new UnauthorizedException("User already exist");

        string hashPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12);

        var register = new RegisterModel()
        {
            Username = dto.Username,
            Email = dto.Email,
            HashPassword = hashPassword
        };

        await _profilesRepository.InsertUser(register);
    }

    public async Task ResetPassword(ResetPasswordDTO dto, Guid UserId)
    {
        var profile = await _profilesRepository.GetProfileById(UserId);

        if (profile == null)
            throw new NotFoundException("User do not exist");

        string hashPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword, workFactor: 12);

        var newData = new ProfileModel
        {
            Id = profile.Id,
            Username = profile.Username,
            Bio = profile.Bio,
            Phone = profile.Phone,
            LogoUrl = profile.LogoUrl,
            CreatedAt = profile.CreatedAt,
            Email = profile.Email,
            Premium = profile.Premium,
            Password = hashPassword
        };

        await _profilesRepository.ModifyProfile(UserId, newData);
    }

    public async Task PasswordRecovery(PasswordRecoveryDTO dto)
    {
        var profile = await _profilesRepository.GetProfileByEmail(dto.Email)
            ?? throw new NotFoundException("User doesnt exist");

        string token = _tokenService.GenerateToken(profile.Id, 1)
            ?? throw new Exception("Can't create JWT");

        string emailBody = $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Reset Your Password</title>
        </head>
        <body style="margin: 0; padding: 0; background-color: #f9f9f9; font-family: Arial, sans-serif; -webkit-font-smoothing: antialiased;">

            <table width="100%" border="0" cellspacing="0" cellpadding="0" style="background-color: #f9f9f9; padding: 40px 0;">
                <tr>
                    <td align="center">
                        
                        <!-- Email Container -->
                        <table width="100%" max-width="600" border="0" cellspacing="0" cellpadding="0" style="max-width: 600px; background-color: #ffffff; border-radius: 8px; border: 1px solid #e0e0e0; box-shadow: 0 4px 6px rgba(0,0,0,0.05); overflow: hidden;">
                            
                            <!-- Header -->
                            <tr>
                                <td style="background-color: #2e7d32; padding: 30px; text-align: center;">
                                    <h1 style="color: #ffffff; margin: 0; font-size: 24px; font-weight: bold; letter-spacing: 0.5px;">Password Recovery</h1>
                                </td>
                            </tr>

                            <!-- Body -->
                            <tr>
                                <td style="padding: 40px 30px; text-align: center;">
                                    <p style="font-size: 16px; line-height: 1.6; color: #333333; margin: 0 0 30px 0;">
                                        You requested to reset your password. Click the button below to secure your account.
                                    </p>
                                    
                                    <!-- Button -->
                                    <table border="0" cellspacing="0" cellpadding="0" style="margin: 0 auto;">
                                        <tr>
                                            <td align="center" style="border-radius: 4px; background-color: #4caf50;">
                                                <a href="{{_config["FRONTEND_URL"]}}reset-password?{{token}}" target="_blank" style="font-size: 16px; font-weight: bold; color: #ffffff; text-decoration: none; padding: 14px 30px; display: inline-block; border-radius: 4px;">
                                                    Reset Password
                                                </a>
                                            </td>
                                        </tr>
                                    </table>

                                    <!-- Alternative Link -->
                                    <p style="font-size: 14px; line-height: 1.5; color: #666666; margin: 30px 0 0 0;">
                                        If the button above does not work, access this link to recovery your password:<br>
                                        <a href="{{_config["FRONTEND_URL"]}}reset-password?{{token}}" target="_blank" style="color: #2e7d32; text-decoration: underline; word-break: break-all; display: inline-block; margin-top: 10px;">
                                            {{_config["FRONTEND_URL"]}}reset-password?{{token}}
                                        </a>
                                    </p>
                                </td>
                            </tr>

                            <!-- Footer -->
                            <tr>
                                <td style="background-color: #f1f8e9; padding: 20px; text-align: center; border-top: 1px solid #e8f5e9;">
                                    <p style="font-size: 12px; color: #558b2f; margin: 0;">
                                        If you did not request this change, please ignore this email safely.
                                    </p>
                                </td>
                            </tr>

                        </table>
                        <!-- End Email Container -->

                    </td>
                </tr>
            </table>

        </body>
        </html>
        """;

        bool isEmailSent = await _sendGridService.SendEmail(
            dto.Email,
            "CatalogoZap: password recovery",
            "Access this link to recovery your password",
            emailBody
        );

        if (!isEmailSent)
        {
            throw new Exception("The Email was not sent");
        }
    }

    public async Task<ProfileModel> GetProfiles(Guid userId)
    {
        return await _profilesRepository.PublicGetProfileById(userId)
            ?? throw new NotFoundException("Profile not found");
    }

    public async Task ModifyProfile(ModifyProfileDTO update, Guid userId)
    {
        var oldProfile = await _profilesRepository.GetProfileById(userId)
            ?? throw new NotFoundException("Profile doesnt exists");

        string? photoUrl = update.Photo != null ? await _cloudinaryService.UploadImageAsync(update.Photo) : null;
        string? password = update.Password != null ? BCrypt.Net.BCrypt.HashPassword(update.Password, workFactor: 12) : null;

        var newData = new ProfileModel
        {
            Id = userId,
            Username = update.Name ?? oldProfile.Username,
            Bio = update.Bio ?? oldProfile.Bio,
            Phone = update.Phone ?? oldProfile.Phone,
            LogoUrl = photoUrl ?? oldProfile.LogoUrl,
            CreatedAt = oldProfile.CreatedAt,
            Email = update.Email ?? oldProfile.Email,
            Premium = oldProfile.Premium,
            Password = password ?? oldProfile.Password
        };

        await _profilesRepository.ModifyProfile(userId, newData);
    }
}
