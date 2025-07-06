using Assets_Api.Database;
using Assets_Api.Database.Repositiries;
using Assets_Api.Services;
using Microsoft.EntityFrameworkCore;


namespace Assets_Api
{
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
            builder.Services.AddHttpClient<FintachartsAuthService>();
            builder.Services.AddHttpClient<AssetsService>();
            builder.Services.AddHttpClient<PriceInfoService>();
            builder.Services.AddScoped<PricesStreamingService>();
            builder.Services.AddScoped<AssetsRepository>();
            builder.Services.AddScoped<PriceInfoRepository>();


            builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetSection("DBConnection")["DefaultConnection"]));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
