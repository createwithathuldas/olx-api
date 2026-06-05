using System.Collections.Concurrent;
using olx_api.Services;

namespace olx_api.Tests
{
    public class TestEmailService : IEmailService
    {
        private readonly ConcurrentDictionary<string, string> _registrationOtps = new();
        private readonly ConcurrentDictionary<string, string> _passwordResetOtps = new();

        public Task SendPasswordResetOtpAsync(string email, string fullName, string otp)
        {
            _passwordResetOtps[email.ToLowerInvariant()] = otp;
            return Task.CompletedTask;
        }

        public Task SendRegistrationOtpAsync(string email, string fullName, string otp)
        {
            _registrationOtps[email.ToLowerInvariant()] = otp;
            return Task.CompletedTask;
        }

        public string? GetRegistrationOtp(string email)
        {
            return _registrationOtps.TryGetValue(email.ToLowerInvariant(), out var otp) ? otp : null;
        }

        public string? GetPasswordResetOtp(string email)
        {
            return _passwordResetOtps.TryGetValue(email.ToLowerInvariant(), out var otp) ? otp : null;
        }
    }
}
