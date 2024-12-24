
using WebApplication1.Packages;

namespace WebApplication1
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

            builder.Services.AddScoped<IPKG_BOOKS, PKG_BOOKS>();
            builder.Services.AddScoped<IPKG_CART, PKG_CART>();
            builder.Services.AddScoped<IPKG_ORDERS, PKG_ORDERS>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllCors", config =>
                {
                    config.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAllCors");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
