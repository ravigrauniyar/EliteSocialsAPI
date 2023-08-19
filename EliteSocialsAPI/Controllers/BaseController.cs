using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Application.ApiResponseService;
using Domain.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Domain.Entities;

namespace EliteSocialsAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public BaseController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        public UserViewModel ClaimValue()
        {
            var userObjectString = _httpContextAccessor!.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == "User")?.Value;
            var userObject = JsonSerializer.Deserialize<UserViewModel>(userObjectString!);
            return userObject!;
        }
        public IActionResult GetTokenResponses<T>(ServiceResult<T> response)
        {
            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
            var userObject = response.data.value;
            var userData = JsonSerializer.Serialize(userObject);
            var claims = new[]
                   {
                            new Claim(JwtRegisteredClaimNames.Iss, jwt!.issuer),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                            new Claim("User",userData)
                        };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.key));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.issuer,
                jwt.audience,
                claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: signIn
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var responseData = new ApiResponseData<string>
            {
                value = tokenString
            };
            var apiResponse = ServiceResult<string>.AsSuccess(responseData);

            return Ok(apiResponse);
        }
        public IActionResult ModelError(ModelStateDictionary modelState)
        {
            var errorMessages = modelState
            .Where(e => e.Value!.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors)
            .Select(err => err.ErrorMessage)
            .ToList();

            if (errorMessages.Count == 1)
            {
                var response = ServiceResult<List<string>>.AsFailure(errorMessages[0]);
                return BadRequest(response);
            }
            var responses = ServiceResult<List<string>>.AsFailure(string.Join(",", errorMessages));
            return BadRequest(responses);
        }
        public IActionResult HandleException(Exception ex)
        {
            int statusCode = ex.HResult & 0xFFFF; // Extract lower 16 bits (status code)
            if (statusCode == 16387)
            {
                var responseError =
                    ServiceResult<string>.AsFailure("Log in to access this feature");
                
                return GetApiResponse(responseError);
            }
            else
            {
                var responseError =
                    ServiceResult<string>.AsFailure(ex.Message);
                
                return GetApiResponse(responseError);
            }
        }
        public IActionResult GetApiResponse<T>(ServiceResult<T> result)
        {
            return (result.isSuccess) ? Ok(result) : BadRequest(result);
        }
    }
}