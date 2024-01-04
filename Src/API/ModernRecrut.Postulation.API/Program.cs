using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ModernRecrut.Postulations.API.Data;
using ModernRecrut.Postulations.API.Interfaces;
using ModernRecrut.Postulations.API.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API ModernRecrut - Postulations & Notes",
        Version = "v1",
        Description = "API d'exemple pour la gestion des postulations " +
        "de la compagnie ModernRecrut",
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

// Energistrer le context
builder.Services.AddDbContext<PostulationContext>(options =>
    options.UseLazyLoadingProxies()
    .UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));

// Ajouter les service des postulations
builder.Services.AddScoped<IGenerateurEvaluation, GenerateurEvaluation>();
builder.Services.AddScoped<IPostulationsService, PostulationsService>();
builder.Services.AddScoped<INotesService, NotesService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
