var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// diccionario para guardar
var memoryDB = new Dictionary<string, string>();

app.UseDefaultFiles(); // buscar automaticamente el archivo index.html
app.UseStaticFiles(); // permite servir archivos de la carpeta wwwroot

// endpoint para acortar la url
app.MapPost("/shorten", (CreateUrlRequest petition) =>
{
   if (string.IsNullOrEmpty(petition.LongUrl))
    {
        return Results.BadRequest("La URL no puede estar vacia.");
    } 

    // generar un codigo aleatorio de 6 caracteres
    string shortCode = Guid.NewGuid().ToString().Substring(0,6);

    // guardarlo en el diccionario
    memoryDB[shortCode] = petition.LongUrl;

    // devolver el codigo al usuario
    return Results.Ok(new { Code = shortCode, shortUrl = $"https://urlshortener-ulaspbit.onrender.com/{shortCode}"});
});

app.MapGet("/{code}", (string code) =>
{
    // buscar el codigo en el diccionario
   if (memoryDB.TryGetValue(code, out var longUrl))
    {
        // si existe, redirigimos al usuario a la web real
        return Results.Redirect(longUrl);
    } 

    // si no existe, devolvemos un error 404
    return Results.NotFound("El codigo no existe") ;
});

app.Run();

public class CreateUrlRequest
{
    public string LongUrl { get; set; } = string.Empty;
}