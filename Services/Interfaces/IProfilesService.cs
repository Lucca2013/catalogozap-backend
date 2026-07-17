using CatalogoZap.DTOs;
using CatalogoZap.Models;

namespace CatalogoZap.Services.Interfaces;

public interface IProfilesService
{
    Task<bool> HasReachedFreeTierLimit(Guid userId);
    Task<string> Login(LoginDTO dto);
    Task Register(RegisterDTO dto);
    Task PasswordRecovery(PasswordRecoveryDTO dto);
    Task<ProfileModel> GetProfiles(Guid UserId);
    Task ModifyProfile(ModifyProfileDTO update, Guid UserId);
}
