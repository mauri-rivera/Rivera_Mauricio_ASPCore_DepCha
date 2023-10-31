using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DebCha.Models;

namespace DebCha.Controllers;

public class HomeController : Controller
{
    public static bool passwordGenerada;

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Hint on a few errors: pay close attention to how things are named

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("user/create")]
    public IActionResult CreateUser(User newUser)
    {
        if (ModelState.IsValid)
        {
            // Apareció un error al intentar guardar la sesión, porque faltó agregar la configuración de inicio de sesión
            HttpContext.Session.SetString("Username", newUser.Name);
            HttpContext.Session.GetString("Username");
            if (newUser.Location != null)
            {
                HttpContext.Session.SetString("Location", $"{newUser.Location}");
            }
            else
            {
                HttpContext.Session.SetString("Location", "Undisclosed");
            }
            return RedirectToAction("Generator");
        }
        else
        {
            return View("Index");
        }
    }

    [HttpGet("generator")]
    //[HttpGet("generate")]
    public IActionResult Generator()
    {
        // Error en el key de la sesion: No reconoce la key "Name" porque no existe
        // if (HttpContext.Session.GetString("Name") == null)
        // La solución es cambiar el nombre de la key "Name" a "Username"
        if (HttpContext.Session.GetString("Username") == null)
        {
            return RedirectToAction("Index");
        }
        if (HttpContext.Session.GetString("Passcode") == null)
        {
            // La solución es llamar primero a la vista "GenerateNew" y luego llamar a la función GeneratePasscode()
            // GeneratePasscode();
            // Por lo tanto, quedaría así:
            return RedirectToAction("GenerateNew");
        }
        return View();
    }

    [HttpPost("reset")]
    public IActionResult Reset()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    // Se produce un error al direccionar a la vista "Generate" porque falta mostrarlo con HttpGet
    // Por lo tanto la solución es:
    [HttpGet("generate/new")]
    public IActionResult GenerateNew()
    {
        // Falto condicionar el acceso a la vista "Generate" para generar una contraseña aleatoria
        if (HttpContext.Session.GetString("Username") == null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            // Se crea un objeto tipo User para generar la primera contraseña aleatoria
            User user = new User
            {
                // Lo que falta: Se incluye la contraseña en el objeto usuario
                Password = GeneratePasscode()
            };
            // Y lo que falta: Se almacena la contraseña para iniciar sesión con setting y getting respectivamente
            HttpContext.Session.SetString("Passcode", user.Password);
            HttpContext.Session.GetString("Passcode");
        }

        return View("Generate");
    }

    // Hint: Something isn't right here...
    // Falta un parámetro del método de acción "GenerateNew" y sería recibir el objeto usuario
    [HttpPost("generate/new")]
    public IActionResult GenerateNew(/* El parámetro faltante: */ User newUser)
    {
        // Lo que falta: Se incluye la contraseña en el objeto usuario
        newUser.Password = GeneratePasscode();
        //GeneratePasscode();
        // Y lo que falta: Se almacena la contraseña para iniciar sesión con setting y getting respectivamente
        HttpContext.Session.SetString("Passcode", newUser.Password);
        HttpContext.Session.GetString("Passcode");

        // No es necesario retornar al método de acción "Generator", sólo basta mostrar la misma vista
        return View("Generate");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Para mostrar la contraseña esta función debe retorna como string
    //public void GeneratePasscode()
    public string GeneratePasscode()
    {
        string passcode = "";
        string CharOptions = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string NumOptions = "0123456789";
        Random rand = new Random();
        for (int i = 1; i < 15; i++)
        {
            int odds = rand.Next(2);
            if (odds == 0)
            {
                passcode += CharOptions[rand.Next(CharOptions.Length)];
            }
            else
            {
                passcode += NumOptions[rand.Next(NumOptions.Length)];
            }
        }

        // Y el setting de sesión debe ser utilizado en el método de acción "GenerateNew" y no acá
        //HttpContext.Session.SetString("Passcode", passcode);
        // porque está función sirve solo pra genera la contraseña con su respectivo retorno
        return passcode;
    }
}
