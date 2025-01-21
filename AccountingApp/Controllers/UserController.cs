using Core.API.DTOs;
using Core.API.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AccountingApp.API.Controllers
{
    [Route("api/v{version:apiversion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        #region Injection

        private readonly IUserServiceAsync _userServiceAsync;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        public UserController(IUserServiceAsync userServiceAsync,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _userServiceAsync = userServiceAsync;
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        #endregion

        #region Register User

        [HttpPost("Register")]
        [EnableRateLimiting("LoginRegisterPolicy")]
        public async Task<IActionResult> Register(UserResgiterDTO userResgiterDTO)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userServiceAsync.RegisterUser(userResgiterDTO);

            if(user == null)
                return BadRequest(new
                {
                    StatuceOfRegistering = false,
                    Message = "Hase issue fo register current user!!"
                });

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var domian = _configuration["AppSettings:Domain"];

            var confrimEmailLink = Url.Action(nameof(ConfrimEmail), "User", new {user.Id, token}
                , protocol: "https", host: domian);

            var eamilDTO = new SendEmailDTO(user.Email, "تاییده حساب کاربری", confrimEmailLink);

            await _emailSender.SendEmailWithGoogleAsync(eamilDTO);

            return Ok(new
            {
                StatuceOfRegistering = true,
                Message = "Successfully Resgistering User"
            });

        }

        #endregion

        #region Confrim Email

        [HttpPost("ConfrimEmail")]
        [EnableRateLimiting("LoginRegisterPolicy")]
        public async Task<IActionResult> ConfrimEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
                return BadRequest(new
                {
                    StatuceOfConfrimEmail = false,
                    Message = "Cannot find the user !!"
                });

            if (user.EmailConfirmed)
            {
                return BadRequest(new
                {
                    StatuceOfConfrimEmail = false,
                    Message = "This email is already confirmed."
                });
            }

            var resultOfConfrimEmaile = await _userManager.ConfirmEmailAsync(user, token);

            if (resultOfConfrimEmaile.Succeeded)
                return Ok(new
                {
                    StatuceOfConfrimEmail = true,
                    Message = "Suucessfully Confrim Email"
                });

            return BadRequest(new
            {
                StatuceOfConfrimEmail = false,
                Message = "Hase issue in confriming email. Pleas try agian later"
            });
        }

        #endregion

        #region Login User

        [HttpPost("Login")]
        [EnableRateLimiting("LoginRegisterPolicy")]
        public async Task<IActionResult> Login(LoginUserDTO loginUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loginStatuce = await _userServiceAsync.LoginUser(loginUserDTO);

            if(loginStatuce.IsSuccess)
                return Ok(loginStatuce);

            return BadRequest(loginStatuce);
        }

        #endregion
    }
}
