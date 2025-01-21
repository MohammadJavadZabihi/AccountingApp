using Core.API.DTOs;
using Core.API.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Core.API.Service
{
    public class UserServiceAsync : IUserServiceAsync
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAuthenticationService _authenticationService;
        public UserServiceAsync(UserManager<IdentityUser> userManager,
                                IAuthenticationService authenticationService,
                                SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _authenticationService = authenticationService;
            _signInManager = signInManager;
        }

        public async Task<ReturnLoginStatuceDTO> LoginUser(LoginUserDTO loginUserDTO)
        {
            IdentityUser? user;

            if (loginUserDTO.IsEmailLogin && !string.IsNullOrEmpty(loginUserDTO.Email))
            {
                user = await _userManager.FindByEmailAsync(loginUserDTO.Email);
            }
            else
            {
                user = await _userManager.FindByNameAsync(loginUserDTO.UserName);
            }

            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, 
                        loginUserDTO.Password, 
                        loginUserDTO.RememberMe,
                        false);

                    if(result.Succeeded)
                    {
                        var token = _authenticationService.GenerateJwtToken(user, loginUserDTO.RememberMe);

                        if (!string.IsNullOrEmpty(token))
                        {
                            return new ReturnLoginStatuceDTO
                            {
                                IsSuccess = true,
                                Message = "Sucessfully logining in app",
                                Token = token
                            };
                        }
                    }
                }
                else
                {
                    return new ReturnLoginStatuceDTO
                    {
                        IsSuccess = false,
                        Message = "Pleas Confrim Your Email Address!!",
                        Token = ""
                    };
                }
            }

            return new ReturnLoginStatuceDTO
            {
                IsSuccess = false,
                Message = "User Not Found!!",
                Token = ""
            };
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
