using API.Infrastructure.Filters;
using API.Infrastructure.Middlewares;
using API.Infrastructure.Services;
using Application;
using Application.Contexts;
using Infrastructure;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddApplication();
//builder.Services.AddInfrastructure(conf);
builder.Services.AddScoped<IMantenimientosRepository, MantenimientosRepository>();
builder.Services.AddScoped<IHorariosRepository, HorariosRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<ExceptionFilterHandler>();
});



builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Environment.IsProduction()
        ? builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")
        : builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            builder.Configuration.GetConnectionString("BaseFrontendURL") ?? "", // url Appservice frontend
            builder.Configuration.GetConnectionString("LocalFrontendURL") ?? "" // desarrollo local
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApiDocument(
    opt =>
    {
        opt.DefaultResponseReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.Null;
        opt.DocumentName = "v1";
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => "OK");

app.UseMiddleware<JwtMiddleware>();

app.Run();
