using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Interfaces.Documents;
using ModernRecrut.MVC.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ModernRecrut.MVC.Models.User;
using ModernRecrut.MVC.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection") ??
    throw new InvalidOperationException("Connection string 'ModernRecrutMVCContextConnection' not found.");

builder.Services.AddDbContext<ModernRecrutMVCContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<Utilisateur>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ModernRecrutMVCContext>();

// Add proxies
builder.Services.AddHttpClient<IOffresEmploisService,
    OffresEmploisServiceProxy>(client => client.BaseAddress = new Uri(
        builder.Configuration.GetValue<string>("UrlAPI_OffresEmplois"))
    );

builder.Services.AddHttpClient<IFavorisService,
    FavorisServiceProxy>(client => client.BaseAddress = new Uri(
        builder.Configuration.GetValue<string>("UrlAPI_Favoris"))
    );

builder.Services.AddHttpClient<IDocumentService,
    DocumentsServiceProxy>(client => client.BaseAddress = new Uri(
        builder.Configuration.GetValue<string>("UrlAPI_Documents"))
    );

builder.Services.AddHttpClient<IPostulationsService,
    PostulationsServiceProxy>(client => client.BaseAddress = new Uri(
        builder.Configuration.GetValue<string>("UrlAPI_Postulations"))
    );

builder.Services.AddHttpClient<INotesService,
    NotesServiceProxy>(client => client.BaseAddress = new Uri(
        builder.Configuration.GetValue<string>("UrlAPI_Notes"))
    );

// Add logging services
builder.Logging.ClearProviders();

builder.Logging.AddConsole();
builder.Logging.AddEventLog();
builder.Logging.Services.AddLogging();

//builder.Services.AddControllersWithViews();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Home/CodeStatus?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
