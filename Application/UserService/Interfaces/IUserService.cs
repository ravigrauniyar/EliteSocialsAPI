using Application.ApiResponseService;
using Domain.Entities;
using Domain.Models;

namespace Application.UserService.Interfaces
{
    public interface IUserService
    {
        public Task<ServiceResult<UserEntity>> GetUser(LoginViewModel loginCredentials);
        public Task<ServiceResult<UserViewModel>> AddUser(SignUpViewModel registerModel);
        public Task<ServiceResult<UserViewModel>> UpdateUser(Guid userId, UpdateUserModel userModel);
        public Task<ServiceResult<string>> SendOTPToUser(Guid userId);
        public Task<ServiceResult<UserViewModel>> CheckOTPForUser(Guid userId, string OTP);
        public Task<ServiceResult<UserViewModel>> ViewUserProfile(Guid userId);
        public Task<ServiceResult<string>> DeleteUserProfile(Guid userId);
    }
}
