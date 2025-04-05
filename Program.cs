namespace SpotifyGoldServer;

using SpotifyGoldServer.Utils;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Tasks
        app.Lifetime.ApplicationStarted.Register(() => {
            Task.Run(async () => {
                while (true) {
                    IO.cleanTempFolder(TimeSpan.FromDays(7));
                    await Task.Delay(TimeSpan.FromHours(1));
                }
            });
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
