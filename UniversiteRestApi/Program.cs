using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.RepositoryFactories;
using MySql.EntityFrameworkCore.Extensions;
using UniversiteDomain.JeuxDeDonnees;
using UniversiteEFDataProvider.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.AddConsole();
});

// Configuration de la connexion à MySql
String connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
    ?? throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");

// Création du contexte de la base de données en utilisant la connexion MySql
builder.Services.AddDbContext<UniversiteDbContext>(options => options.UseMySQL(connectionString));

// La factory est rajoutée dans les services de l'application
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

// Sécurisation
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

// IMPORTANT: Identity configuration for UniversiteUser *and* UniversiteRole
builder.Services.AddIdentityCore<UniversiteUser>()
    .AddRoles<UniversiteRole>() // <-- Changed from IdentityRole to UniversiteRole
    .AddEntityFrameworkStores<UniversiteDbContext>()
    .AddApiEndpoints();

var app = builder.Build();

// Configuration du serveur Web
app.UseHttpsRedirection();
app.MapControllers();

// Configuration de Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Sécurisation
app.UseAuthorization();
// Ajoute les points d'entrée dans l'API pour s'authentifier, se connecter et se déconnecter
app.MapIdentityApi<UniversiteUser>();

// Initialisation de la base de données
using(var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<UniversiteDbContext>>();
    DbContext context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    logger.LogInformation("Initialisation de la base de données");
    
    logger.LogInformation("Suppression de la BD si elle existe");
    await context.Database.EnsureDeletedAsync();

    logger.LogInformation("Création de la BD et des tables à partir des entities");
    await context.Database.EnsureCreatedAsync();
}

// Chargement des données de test
ILogger logger2 = app.Services.GetRequiredService<ILogger<BdBuilder>>();
logger2.LogInformation("Chargement des données de test");
using(var scope = app.Services.CreateScope())
{
    UniversiteDbContext context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    IRepositoryFactory repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();   
    BdBuilder seedBD = new BasicBdBuilder(repositoryFactory);
    await seedBD.BuildUniversiteBdAsync();
}

// Exécution de l'application
app.Run();
