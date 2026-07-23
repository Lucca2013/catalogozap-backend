namespace CatalogoZap.DTOs
{
    public record ModifyProfileDTO
    {
        public string? Name {get; set;}

        public string? Bio {get; set;}

        public string? Phone {get; set;}

        public string? Email {get; set;}

        public string? Password {get; set;}

        public IFormFile? Photo {get; set; }

    }
}