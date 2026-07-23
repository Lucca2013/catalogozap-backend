using System.ComponentModel.DataAnnotations;

namespace CatalogoZap.Attributes;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxBytes;

    public MaxFileSizeAttribute(int maxBytes) {
        _maxBytes = maxBytes;
        ErrorMessage = $"File size cannot exceed {maxBytes} bytes.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
        if (value is IFormFile file && file.Length > _maxBytes) {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
