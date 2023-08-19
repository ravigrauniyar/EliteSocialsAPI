using Domain.Entities;
using Domain.Models;

namespace Application.UserService.Interfaces
{
    public interface IUserRepository
    {
        public Task<UserEntity?> GetUserIfExists(LoginViewModel loginCredentials);
        public Task<UserViewModel?> AddUserToDb(SignUpViewModel registerModel);
        public Task<UserViewModel?> UpdateUserInDb(Guid userId, UpdateUserModel userModel);
        public Task<UserEntity?> CheckUserByEmail(string email);
        public Task<UserEntity?> CheckUserById(Guid userId);
        public bool SendOTPMail(string? name, string? to, string? body);
        public Task<bool> GenerateAndSendOTP(UserEntity user);
        public Task<bool> VerifyOTPInDB(Guid id, string otp);
        public Task<bool> DeleteUserById(Guid id);
    }
}
