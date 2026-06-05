using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using olx_api.DTOs;
using Xunit;

namespace olx_api.Tests
{
    public class AuthTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Register_Verify_Login_ProfileFlow_Works()
        {
            var client = _factory.CreateClient();
            var email = "testuser@example.com";
            var phone = "+15550000001";

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(
                FullName: "Test User",
                Email: email,
                Password: "Password1!",
                ConfirmPassword: "Password1!",
                PhoneNumber: phone));

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var otp = _factory.EmailService.GetRegistrationOtp(email);
            Assert.False(string.IsNullOrWhiteSpace(otp));

            var verifyResponse = await client.PostAsJsonAsync("/api/auth/verify-otp", new VerifyOtpDto(email, otp));
            Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);

            var auth = await verifyResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.NotNull(auth);
            Assert.Equal(email, auth!.Email);
            Assert.False(string.IsNullOrWhiteSpace(auth.Token));
            Assert.False(string.IsNullOrWhiteSpace(auth.RefreshToken));

            var loginByEmail = await client.PostAsJsonAsync("/api/auth/login", new LoginDto(email, "Password1!"));
            Assert.Equal(HttpStatusCode.OK, loginByEmail.StatusCode);

            var loginResult = await loginByEmail.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.NotNull(loginResult);
            Assert.Equal(email, loginResult!.Email);

            var loginByPhone = await client.PostAsJsonAsync("/api/auth/login", new LoginDto(phone, "Password1!"));
            Assert.Equal(HttpStatusCode.OK, loginByPhone.StatusCode);

            var phoneLoginResult = await loginByPhone.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.NotNull(phoneLoginResult);
            Assert.Equal(email, phoneLoginResult!.Email);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

            var profileResponse = await client.GetAsync("/api/profile");
            Assert.Equal(HttpStatusCode.OK, profileResponse.StatusCode);

            var profile = await profileResponse.Content.ReadFromJsonAsync<UserProfileDto>();
            Assert.NotNull(profile);
            Assert.Equal(email, profile!.Email);
            Assert.Equal("Test User", profile.FullName);

            var updateResponse = await client.PutAsJsonAsync("/api/profile", new UpdateProfileDto("Updated User", "+15550000002", null));
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updatedProfile = await updateResponse.Content.ReadFromJsonAsync<UserProfileDto>();
            Assert.NotNull(updatedProfile);
            Assert.Equal("Updated User", updatedProfile!.FullName);
            Assert.Equal("+15550000002", updatedProfile.PhoneNumber);

            var changePasswordResponse = await client.PutAsJsonAsync("/api/profile/change-password", new ChangePasswordDto("Password1!", "NewPassword1!", "NewPassword1!"));
            Assert.Equal(HttpStatusCode.NoContent, changePasswordResponse.StatusCode);

            client.DefaultRequestHeaders.Authorization = null;
            var loginWithNewPassword = await client.PostAsJsonAsync("/api/auth/login", new LoginDto(email, "NewPassword1!"));
            Assert.Equal(HttpStatusCode.OK, loginWithNewPassword.StatusCode);
        }

        [Fact]
        public async Task Register_WithShortPassword_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(
                FullName: "Short Password",
                Email: "shortpass@example.com",
                Password: "Pass1",
                ConfirmPassword: "Pass1",
                PhoneNumber: "+15550000003"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_DuplicateEmailOrPhone_ReturnsConflict()
        {
            var client = _factory.CreateClient();
            var email = "duplicate@example.com";
            var phone = "+15550000004";

            var response1 = await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(
                FullName: "Duplicate User",
                Email: email,
                Password: "Password1!",
                ConfirmPassword: "Password1!",
                PhoneNumber: phone));

            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

            var response2 = await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(
                FullName: "Duplicate Email",
                Email: email,
                Password: "Password1!",
                ConfirmPassword: "Password1!",
                PhoneNumber: "+15550000005"));

            Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);

            var response3 = await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(
                FullName: "Duplicate Phone",
                Email: "unique@example.com",
                Password: "Password1!",
                ConfirmPassword: "Password1!",
                PhoneNumber: phone));

            Assert.Equal(HttpStatusCode.Conflict, response3.StatusCode);
        }

        [Fact]
        public async Task ForgotPassword_AndResetPassword_Works()
        {
            var client = _factory.CreateClient();
            var email = "forgotpassword@example.com";
            var phone = "+15550000006";

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(
                FullName: "Forgot Password",
                Email: email,
                Password: "Password1!",
                ConfirmPassword: "Password1!",
                PhoneNumber: phone));

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var otp = _factory.EmailService.GetRegistrationOtp(email);
            Assert.False(string.IsNullOrWhiteSpace(otp));

            var verifyResponse = await client.PostAsJsonAsync("/api/auth/verify-otp", new VerifyOtpDto(email, otp));
            Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);

            var forgotResponse = await client.PostAsJsonAsync("/api/auth/forgot-password", new ForgotPasswordDto(email));
            Assert.Equal(HttpStatusCode.OK, forgotResponse.StatusCode);

            var resetOtp = _factory.EmailService.GetPasswordResetOtp(email);
            Assert.False(string.IsNullOrWhiteSpace(resetOtp));

            var resetResponse = await client.PostAsJsonAsync("/api/auth/reset-password", new ResetPasswordDto(email, resetOtp!, "ResetPassword1!"));
            Assert.Equal(HttpStatusCode.NoContent, resetResponse.StatusCode);

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginDto(email, "ResetPassword1!"));
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        }
    }
}
