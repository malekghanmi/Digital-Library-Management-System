using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using BibliothequeAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// === SERVICES ===
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Bibliothèque API",
        Version = "v1",
        Description = "API REST pour la gestion complète de la bibliothèque"
    });
});

// === DATABASE (Code First avec SQL Server) ===
builder.Services.AddDbContext<BibliothequeContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BibliothequeDB"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(3)
    )
    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
);

// === CORS - Autoriser le projet Front ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFront", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7100",  // Front HTTPS
                "http://localhost:5100"    // Front HTTP
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// === MIGRATION AUTOMATIQUE AU DÉMARRAGE ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BibliothequeContext>();
    db.Database.Migrate();
}

// === MIDDLEWARE ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bibliothèque API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFront");
app.UseAuthorization();
app.MapControllers();

app.Run();