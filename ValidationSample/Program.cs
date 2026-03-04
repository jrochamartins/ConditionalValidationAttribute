
using Scalar.AspNetCore;
using ValidationSample.Filters;

namespace ValidationSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.           

            builder.Services.AddControllers();
            builder.Services.AddOpenApi(options =>
            {
                options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
                options.AddSchemaTransformer<OpenApiSchemaFilter>();
            });

            //builder.Services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    // Desabilita o retorno automático de 400 Bad Request
            //    options.SuppressModelStateInvalidFilter = true;
            //});

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.MapScalarApiReference();
                app.UseSwaggerUI(options =>
                {
                    options.DisplayRequestDuration();
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                });
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
