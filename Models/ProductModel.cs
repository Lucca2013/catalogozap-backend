using System.ComponentModel.DataAnnotations;

namespace CatalogoZap.Models;

public record ProductModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StoreId { get; set; }
    public required string Name { get; set; }
    public decimal PriceCents { get; set; }
    public required string PhotoUrl { get; set; }
    public bool Avaliable { get; set; }
    public required string Created_at { get; set; }

}
