namespace Api.Models.DTO.Wishlists;

public class CreateWishlistItemRequest
{
  public Guid WishlistId { get; set; }
  public Guid ProductId { get; set; }
}
