using System.ComponentModel.DataAnnotations;

namespace ImageStorage.Application.Requests;

public class RegisterUserRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public override string ToString()
    {
        return $"{nameof(RegisterUserRequest)}: Name={Name};Email={Email};";
    }
}
