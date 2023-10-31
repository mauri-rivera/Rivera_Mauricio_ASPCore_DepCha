#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace DebCha.Models;

public class User
{
    [Required]
    public string Name { get; set; }

    // Para iniciar sesión, todo nombre de usuario necesita una contraseña, 
    // y en este caso, falta uno, por lo tanto la solución más obvia es agregarlo
    public string? Password { get; set; }

    public string? Location { get; set; }
}