
using API.Extenstions;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);




        //this will call the extension methods we've created 
        builder.Services.AddApplicationServices(builder.Configuration);

        //JWT creation method 
        builder.Services.AddIdentityServices(builder.Configuration);


       
        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
        .WithOrigins("http://localhost:4200", "https://localhost:4200"));
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}
