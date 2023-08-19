using Application.UserService.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EliteSocialsAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]/")]
    public class UserController: Controller
    {
        private readonly IUserService _userService;
        private readonly BaseController _baseController;
        public UserController(IUserService userService, BaseController baseController)
        {
            _userService = userService;
            _baseController = baseController;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var apiResponse = await _userService.GetUser(loginModel);
                    if (apiResponse.isSuccess)
                    {
                        return _baseController.GetTokenResponses(apiResponse);
                    }
                    return _baseController.GetApiResponse(apiResponse);
                }
                else
                {
                    return _baseController.ModelError(ModelState);
                }
            }
            catch (Exception ex)
            {
                return _baseController.HandleException(ex);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] SignUpViewModel registerModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var apiResponse = await _userService.AddUser(registerModel);
                    return _baseController.GetApiResponse(apiResponse);
                }
                else
                {
                    return _baseController.ModelError(ModelState);
                }
            }
            catch (Exception ex)
            {
                return _baseController.HandleException(ex);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("{userId}/update")]
        public async Task<IActionResult> UpdateProfile([FromRoute] Guid userId, [FromBody] UpdateUserModel updateUserModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var apiResponse = await _userService.UpdateUser(userId, updateUserModel);
                    return _baseController.GetApiResponse(apiResponse);
                }
                else
                {
                    return _baseController.ModelError(ModelState);
                }
            }
            catch (Exception ex)
            {
                return _baseController.HandleException(ex);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("enableTFA")]
        public async Task<IActionResult> SendOTP()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var apiResponse = await _userService.SendOTPToUser(_baseController.ClaimValue().userId);
                    return _baseController.GetApiResponse(apiResponse);
                }
                else
                {
                    return _baseController.ModelError(ModelState);
                }
            }
            catch (Exception ex)
            {
                return _baseController.HandleException(ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("verifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody] string OTP)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var apiResponse = await _userService.CheckOTPForUser(_baseController.ClaimValue().userId, OTP);
                    return _baseController.GetApiResponse(apiResponse);
                }
                else
                {
                    return _baseController.ModelError(ModelState);
                }
            }
            catch (Exception ex)
            {
                return _baseController.HandleException(ex);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("profile")]
        public async Task<IActionResult> ViewProfile()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var apiResponse = await _userService.ViewUserProfile(_baseController.ClaimValue().userId);
                    return _baseController.GetApiResponse(apiResponse);
                }
                else
                {
                    return _baseController.ModelError(ModelState);
                }
            }
            catch (Exception ex)
            {
                return _baseController.HandleException(ex);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("{userId}/delete")]
        public async Task<IActionResult> DeleteProfile(Guid userId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var apiResponse = await _userService.DeleteUserProfile(_baseController.ClaimValue().userId);
                    return _baseController.GetApiResponse(apiResponse);
                }
                else
                {
                    return _baseController.ModelError(ModelState);
                }
            }
            catch (Exception ex)
            {
                return _baseController.HandleException(ex);
            }
        }
    }
}
