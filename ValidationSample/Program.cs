
using Scalar.AspNetCore;
using ValidationSample.OpenApi;

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

                options.AddSchemaTransformer((schema, context, ct) =>
                {
                    if (context.JsonTypeInfo.Type.FullName is not null)
                    {
                        // FORMA NATIVA E CORRETA NO ASP.NET 10:
                        // Resolve o conflito usando o namespace completo do tipo
                        options.CreateSchemaReferenceId = (typeInfo) => typeInfo.Type.FullName;
                    }
                    return Task.CompletedTask;
                });

                options.AddSchemaTransformer<OpenApiSchemaTransformer2>();
                //options.AddSchemaTransformer<CachedOpenApiSchemaTransformer>();
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
