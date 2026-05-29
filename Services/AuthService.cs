using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using Microsoft.AspNetCore.Http;

namespace AutoHub.Services
{
    public interface IAuthService
    {
        Task<(bool success, User? user, string message)> LoginAsync(string loginInput, string password);
        Task<(bool success, string message)> RegisterAsync(User user, string password, string confirmPassword);
        void SetUserSession(ISession session, User user);
        void ClearSession(ISession session);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<(bool success, User? user, string message)> LoginAsync(string loginInput, string password)
        {
            var user = await _userRepository.GetByEmailOrPhoneAsync(loginInput);
            if (user == null)
            {
                return (false, null, "Tài khoản không tồn tại!");
            }

            if (user.PasswordHash != HashPassword(password))
            {
                return (false, null, "Mật khẩu không chính xác!");
            }

            return (true, user, "Đăng nhập thành công!");
        }

        public async Task<(bool success, string message)> RegisterAsync(User user, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return (false, "Mật khẩu xác nhận không khớp!");
            }

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                return (false, "Vui lòng nhập Tên đăng nhập!");
            }

            if (await _userRepository.IsUsernameTakenAsync(user.Username, 0))
            {
                return (false, "Tên đăng nhập này đã có người sử dụng!");
            }

            if (!string.IsNullOrWhiteSpace(user.Email) && await _userRepository.IsEmailTakenAsync(user.Email, 0))
            {
                return (false, "Email này đã được sử dụng!");
            }

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber) && await _userRepository.IsPhoneTakenAsync(user.PhoneNumber, 0))
            {
                return (false, "Số điện thoại này đã được sử dụng!");
            }

            user.PasswordHash = HashPassword(password);
            
            // Xử lý mặc định nếu chưa nhập địa chỉ (Cập nhật sau ở trang Account)
            user.StreetName = string.IsNullOrWhiteSpace(user.StreetName) ? "Chưa cập nhật" : user.StreetName;
            user.Ward = string.IsNullOrWhiteSpace(user.Ward) ? "Chưa cập nhật" : user.Ward;
            user.District = string.IsNullOrWhiteSpace(user.District) ? "Chưa cập nhật" : user.District;
            user.City = string.IsNullOrWhiteSpace(user.City) ? "Chưa cập nhật" : user.City;
            
            await _userRepository.AddAsync(user);

            return (true, "Đăng ký thành công!");
        }

        public void SetUserSession(ISession session, User user)
        {
            session.SetInt32("UserId", user.Id);
            session.SetString("UserName", $"{user.LastName} {user.FirstName}");
        }

        public void ClearSession(ISession session)
        {
            session.Clear();
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexStringLower(bytes);
        }
    }
}
