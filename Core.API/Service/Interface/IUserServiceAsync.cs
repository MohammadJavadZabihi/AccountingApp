using Core.API.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Core.API.Service.Interface
{
    public interface IUserServiceAsync
    {
        Task<IdentityUser> RegisterUser(UserResgiterDTO userResgiterDTO);
    }
}
