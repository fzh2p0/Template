using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebAPITemplate.Middleware;
using WebAPITemplate.Models;
using WebAPITemplate.AppData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using WebAPITemplate.ExtensionMethods;

namespace WebAPITemplate
{
    public class Program
    {
        private readonly IHostEnvironment _hostEnvironment;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddValidatorsFromAssemblyContaining<ProductValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ProductOptionValidator>();

            builder.Services.AddEndpointsApiExplorer();

            if (builder.Environment.IsIsolated())
            {
                var databaseName = Guid.NewGuid().ToString();
                builder.Services.AddDbContext<ProductContext>(
                    options => options.UseInMemoryDatabase(databaseName)
                                      // The db data will be changed by functional tests.
                                      .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));
            }
            else
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "AppData");
                var productsConnectionString = builder.Configuration.GetConnectionString("ProductsDatabase")
                    ?.Replace("[DataDirectory]", path);
                builder.Services.AddDbContext<ProductContext>(
                    options => options.UseSqlServer(productsConnectionString,
                            sqlServerOptions => sqlServerOptions.EnableRetryOnFailure())
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            }

            builder.Services.AddTransient(typeof(ProductDataAccess));
            builder.Services.AddTransient(typeof(ProductOptionDataAccess));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<ErrorWrappingMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
