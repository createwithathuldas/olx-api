// DTOs/UserDtos.cs
namespace olx_api.DTOs
{
    public record RegisterDto(string FullName, string Email, string Password, string PhoneNumber);
    public record LoginDto(string Email, string Password);
    public record AuthResponseDto(string Token, string Email, string FullName);
    public record UserProfileDto(Guid Id, string FullName, string Email, string PhoneNumber, DateTime CreatedAt);
}