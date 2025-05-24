using HospitalApp.Core.Models;
using HospitalApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HospitalApp.WPF.Services
{
    public interface IAuthenticationService
    {
        User? CurrentUser { get; }
        Task<bool> LoginAsync(string username, string password);
        void Logout();
        Task<bool> RegisterUserAsync(User user, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        bool IsAuthenticated { get; }
        UserRole CurrentUserRole { get; }
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private User? _currentUser;

        public User? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public UserRole CurrentUserRole => _currentUser?.Role ?? UserRole.Patient;

        public AuthenticationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
            {
                return false;
            }

            if (!user.IsActive)
            {
                return false;
            }

            _currentUser = user;
            user.LastLogin = DateTime.Now;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public async Task<bool> RegisterUserAsync(User user, string password)
        {
            // Check if username or email already exists
            if (await _unitOfWork.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email))
            {
                return false;
            }

            user.PasswordHash = HashPassword(password);
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null || !VerifyPasswordHash(currentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = HashPassword(newPassword);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }
    }
}
