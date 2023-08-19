using Application.ApiResponseService;
using Application.UserService.Interfaces;
using Domain.Entities;
using Domain.Models;

namespace Application.UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ServiceResult<UserEntity>> GetUser(LoginViewModel loginCredentials)
        {
            var response = await _userRepository.GetUserIfExists(loginCredentials);

            if (response != null)
            {
                if (response.username != string.Empty)
                {
                    var responseData = new ApiResponseData<UserEntity>
                    {
                        value = response
                    };
                    return ServiceResult<UserEntity>.AsSuccess(responseData);
                }
                else
                {
                    return ServiceResult<UserEntity>.AsFailure("Incorrect password!");
                }
            }
            return ServiceResult<UserEntity>.AsFailure("User doesn't exist!");
        }
        public async Task<ServiceResult<UserViewModel>> AddUser(SignUpViewModel registerModel)
        {
            var prevUserWithEmail = await _userRepository.CheckUserByEmail(registerModel.email);

            if (prevUserWithEmail == null)
            {
                var response = await _userRepository.AddUserToDb(registerModel);
                if (response != null)
                {
                    var responseData = new ApiResponseData<UserViewModel>
                    {
                        value = response
                    };
                    return ServiceResult<UserViewModel>.AsSuccess(responseData);
                }
                return ServiceResult<UserViewModel>.AsFailure("Username unavailable!");
            }
            return ServiceResult<UserViewModel>.AsFailure("Email unavailable!");
        }
        public async Task<ServiceResult<UserViewModel>> UpdateUser(Guid userId, UpdateUserModel userModel)
        {
            var prevUserWithEmail = await _userRepository.CheckUserByEmail(userModel.email);

            if (prevUserWithEmail == null)
            {
                var response = await _userRepository.UpdateUserInDb(userId, userModel);
                if (response != null)
                {
                    var responseData = new ApiResponseData<UserViewModel>
                    {
                        value = response
                    };
                    return ServiceResult<UserViewModel>.AsSuccess(responseData);
                }
                return ServiceResult<UserViewModel>.AsFailure("Username unavailable!");
            }
            return ServiceResult<UserViewModel>.AsFailure("Email unavailable!");
        }
        public async Task<ServiceResult<string>> SendOTPToUser(Guid userId)
        {
            var user = await _userRepository.CheckUserById(userId);
            if (user != null)
            {
                if (user.email != string.Empty)
                {
                    var isOTPSent = await _userRepository.GenerateAndSendOTP(user);

                    if (isOTPSent)
                    {
                        var response = new ApiResponseData<string>
                        {
                            value = "OTP sent successfully!"
                        };
                        return ServiceResult<string>.AsSuccess(response);
                    }
                    else return ServiceResult<string>.AsFailure("Mail server error occurred!");
                }
                else return ServiceResult<string>.AsFailure("Set email in profile first!");
            }
            else return ServiceResult<string>.AsFailure("User doesn't exist!");
        }
        public async Task<ServiceResult<UserViewModel>> CheckOTPForUser(Guid userId, string OTP)
        {
            var otpResponse = await _userRepository.VerifyOTPInDB(userId, OTP);
            if(otpResponse)
            {
                var user = await _userRepository.CheckUserById(userId);
                var userViewModel = new UserViewModel
                {
                    fullName = user!.fullName,
                    userId = userId,
                    username = user.username,
                    isTFAEnabled = (user.otp != string.Empty),
                    email = user.email,
                    createdAt = user.createdAt
                };
                var response = new ApiResponseData<UserViewModel>
                {
                    value = userViewModel
                };
                return ServiceResult<UserViewModel>.AsSuccess(response);
            }
            return ServiceResult<UserViewModel>.AsFailure("OTP didn't match!");
        }
        public async Task<ServiceResult<UserViewModel>> ViewUserProfile(Guid userId)
        {
            var user = await _userRepository.CheckUserById(userId);
            if (user != null)
            {
                var userView = new UserViewModel
                {
                    userId = userId,
                    username = user!.username,
                    fullName = user.fullName,
                    email = user.email,
                    isTFAEnabled = (user.otp != string.Empty),
                    createdAt = user.createdAt
                };

                var responseData = new ApiResponseData<UserViewModel>
                {
                    value = userView
                };

                return ServiceResult<UserViewModel>.AsSuccess(responseData);
            }
            else return ServiceResult<UserViewModel>.AsFailure("User doesn't exist!");
        }
        public async Task<ServiceResult<string>> DeleteUserProfile(Guid userId)
        {
            var isUserDeleted = await _userRepository.DeleteUserById(userId);

            var responseData = new ApiResponseData<string>
            {
                value = "User deleted successfully!"
            };
            return (isUserDeleted) ? ServiceResult<string>.AsSuccess(responseData) :
                            ServiceResult<string>.AsFailure("User couldn't be deleted!");
        }
    }
}
