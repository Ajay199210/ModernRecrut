using Microsoft.OpenApi.Models;
using ModernRecrut.Favoris.API.Interfaces;
using ModernRecrut.Favoris.API.Services;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 5000000;
});

builder.Services.AddControllers();
  

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API ModernRecrut - Favoris",
        Version = "v1",
        Description = "API d'exemple pour la gestion des favoris des " +
        "offres d'emplois de la compagnie ModernRecrut",
        License = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Url = new Uri("http://www.apache.org")
        },
        Contact = new OpenApiContact
        {
            Name = "Groupe ModernRecrut",
            Email = "grp@modern-recrut.ca",
            Url = new Uri("https://google.com/")
        },
    });

    // Activation du support des commentaires XML dans Swagger UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IFavorisService, FavorisService>();
builder.Services.AddScoped<IUtils, Utils>();

JsonSerializerOptions options = new()
{
    ReferenceHandler = ReferenceHandler.IgnoreCycles
};

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
