// DTOs/ListingDtos.cs
namespace olx_api.DTOs
{
    public record CategoryDto(int Id, string Name, string IconUrl, int? ParentCategoryId);

    public record CreateListingDto(string Title, string Description, decimal Price, bool IsNegotiable, string State, string City, int CategoryId);
    public record UpdateListingDto(string Title, string Description, decimal Price, bool IsNegotiable, string State, string City, string Status);

    public record ListingImageDto(Guid Id, string ImageUrl, bool IsPrimary);
    
    public record ListingResponseDto(
        Guid Id, string Title, string Description, decimal Price, bool IsNegotiable, 
        string State, string City, string Status, DateTime CreatedAt,
        Guid UserId, string SellerName, string SellerPhone,
        CategoryDto Category, IEnumerable<ListingImageDto> Images
    );
}