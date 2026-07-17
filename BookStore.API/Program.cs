using BookStore.API.Middlewares;
using BookStore.Application;
using BookStore.Application.Common.Interfaces;
using BookStore.Infrastructure;

namespace BookStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add application services
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructure(builder.Configuration);


            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Đăng ký Global Exception Middleware
         
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();
          
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapControllers();

            app.MapGet("/hello", () => "Hello World!");

            app.Run();
        }
    }
}
