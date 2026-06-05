namespace olx_api.DTOs
{
    // Auth Updates
    public record ForgotPasswordDto(string Email);
    public record ResetPasswordDto(string Email, string Otp, string NewPassword);
    public record RefreshTokenRequestDto(string Token, string RefreshToken);
    
    // Listing Updates
    // NOTE: CreateListingDto is already defined in ListingDto.cs; do not duplicate it here.
    
    // Admin & Moderation
    public record CreateReportDto(Guid ReportedListingId, string Reason);
    public record AdminDashboardStatsDto(int TotalUsers, int ActiveAds, int PendingAds, int TotalReports);
    public record BannerDto(int Id, string ImageUrl, string PlacementType);
}