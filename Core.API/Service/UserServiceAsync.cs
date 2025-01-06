using Core.API.DTOs;
using Core.API.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Core.API.Service
{
    public class UserServiceAsync : IUserServiceAsync
    {
        private readonly UserManager<IdentityUser> _userManager;
        public UserServiceAsync(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityUser> RegisterUser(UserResgiterDTO userResgiterDTO)
        {
            var registerUser = new IdentityUser
            {
                Email = userResgiterDTO.Email,
                UserName = userResgiterDTO.UserName,
                PhoneNumber = userResgiterDTO.PhoneNumber,
            };
            
            var registerResult = await _userManager.CreateAsync(registerUser, userResgiterDTO.Password);

            if (registerResult.Succeeded)
            { 
                return registerUser;
            }
            
            foreach (var error in registerResult.Errors)
            {
                throw new Exception(error.ToString());
            }

            return null;

        }
    }
}
