using System.ComponentModel.DataAnnotations;
using CatalogoZap.Attributes;

namespace CatalogoZap.DTOs;

public record ProductDTO
{
    [Required] public Guid StoreId { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public decimal PriceCents { get; set; }
    [Required] public bool Avaliable { get; set; }

    [Required]
    [MaxFileSize(6 * 1024 * 1024)]
    public required IFormFile Photo { get; set; }

}

public record ModProductsDTO
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string? Name { get; set; }
    public decimal? PriceCents { get; set; }
    public bool? Avaliable { get; set; }
    
    [MaxFileSize(6 * 1024 *1024)]
    public IFormFile? Photo { get; set; }
}