using API.Infrastructure.Services;
using Infrastructure;
using Application;
using API.Infrastructure.Filters;
using Infrastructure.Interfaces;
using API.Infrastructure.Middlewares;


var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(conf);
builder.Services.AddScoped<IMantenimientosRepository, MantenimientosRepository>();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<ExceptionFilterHandler>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddOpenApiDocument(
    opt =>
    {
        opt.DefaultResponseReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.Null;
        opt.DocumentName = "v1";
    }
);






var intevaloRevision = Convert.ToInt32(conf.GetSection("IntervaloRevision").Value ?? "8");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => "OK");

app.UseMiddleware<JwtMiddleware>();

app.Run();
