using System.ComponentModel.DataAnnotations;

namespace ImageStorage.Application.RequestModels;

public class UserRegisterRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string Email { get; set; }

    public override string ToString()
    {
        return $"Name={Name};Email={Email};";
    }
}
