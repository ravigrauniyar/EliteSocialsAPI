using Application.UserService.Interfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Cryptography;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _databaseContext;
        public UserRepository(DatabaseContext databaseContext) 
        {
            _databaseContext = databaseContext;
        }
        public async Task<UserEntity?> CheckUserByEmail(string email)
        {
            var user = await _databaseContext.TblUsers.FirstOrDefaultAsync(x => x.email == email && email != "");
            return user;
        }
        public async Task<UserEntity?> CheckUserById(Guid userId)
        {
            var user = await _databaseContext.TblUsers.FindAsync(userId);
            return user;
        }
        public bool SendOTPMail(string? name, string? to, string? body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("test@sourcecodespoint.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = "Welcome to EliteSocials - Two Factor Authentication";

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $@"Dear {name},<br><br>
                 Welcome to EliteSocials!<br><br>
                 
                 Your OTP for Two Factor Authentication is: <b>{body}</b><br><br>
                 To ensure the security of your account, we recommend that you enter the provided OTP to log in.<br><br>
                 If you have any questions or need further assistance, our support team is here to help.<br><br>
                 Thank you for choosing EliteSocials. We look forward to serving you!<br><br>
                 Best regards,<br>
                 EliteSocials."
            };

            using var smtp = new SmtpClient();

            smtp.Connect("mail.sourcecodespoint.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("test@sourcecodespoint.com", "nepal@123");
            try
            {
                smtp.Send(email);
                smtp.Disconnect(true);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<UserEntity?> GetUserIfExists(LoginViewModel loginCredentials)
        {
            var user = await _databaseContext.TblUsers.FirstOrDefaultAsync(a => a.username == loginCredentials.username);
            if (user != null)
            {
                if (user.password == loginCredentials.password)
                {
                    return user;
                }
                else return new UserEntity();
            }
            else return default;
        }
        public async Task<UserViewModel?> AddUserToDb(SignUpViewModel registerModel)
        {
            var preExistingUser = await _databaseContext.TblUsers.FirstOrDefaultAsync(a => a.username == registerModel.username);

            if (preExistingUser == null)
            {
                var newUser = new UserEntity
                {
                    userId = Guid.NewGuid(),
                    fullName = registerModel.fullName,
                    username = registerModel.username,
                    password = registerModel.password,
                    email = registerModel.email
                };

                var user = new UserViewModel
                {
                    userId = newUser.userId,
                    username = newUser.username,
                    fullName = newUser.fullName,
                    createdAt = newUser.createdAt
                };

                if (registerModel.email != "")
                {
                    user.email = registerModel.email;
                }
                await _databaseContext.AddAsync(newUser);
                await _databaseContext.SaveChangesAsync();

                return user;
            }
            return default;
        }
        public async Task<UserViewModel?> UpdateUserInDb(Guid userId, UpdateUserModel userModel)
        {
            var preExistingUser = await _databaseContext.TblUsers.FirstOrDefaultAsync(a => a.username == userModel.username);

            if (preExistingUser == null || preExistingUser.userId == userId)
            {
                var user = await _databaseContext.TblUsers.FindAsync(userId);
                
                user!.username = userModel.username;
                user.email = userModel.email;
                user.fullName = userModel.fullName;

                await _databaseContext.SaveChangesAsync();

                var userView = new UserViewModel
                {
                    userId = user.userId,
                    username = user.username,
                    fullName = user.fullName,
                    createdAt = user.createdAt,
                    isTFAEnabled = (user.otp != string.Empty)
                };

                if(user.email != "")
                {
                    userView.email = user.email;
                }
                return userView;
            }
            return default;
        }
        public async Task<bool> GenerateAndSendOTP(UserEntity user)
        {
            using RandomNumberGenerator rand = RandomNumberGenerator.Create();
            
            byte[] bytes = new byte[12];
            rand.GetBytes(bytes);
            string otp = Convert.ToBase64String(bytes);

            var isOTPSent = SendOTPMail(user.fullName, user.email, otp);
            if (isOTPSent)
            {
                user.otp = otp;
                await _databaseContext.SaveChangesAsync();
            }
            return isOTPSent;
        }
        public async Task<bool> VerifyOTPInDB(Guid id, string otp)
        {
            var user = await CheckUserById(id);
            if(user!.otp == otp)
            {
                return true;
            }
            else
            {
                user.otp = string.Empty;
                await _databaseContext.SaveChangesAsync();

                return false;
            }
        }
        public async Task<bool> DeleteUserById(Guid id)
        {
            var user = await CheckUserById(id);
            if (user != null)
            {
                _databaseContext.Remove(user);
                await _databaseContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
