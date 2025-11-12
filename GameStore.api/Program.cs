using GameStore.api.Data;
using GameStore.api.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("Games", new() { Title = "Games API", Version = "v1" });
    options.SwaggerDoc("Genres", new() { Title = "Genres API", Version = "v1" });

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        var groupName = apiDesc.GroupName ?? string.Empty;
        return groupName.Equals(docName, StringComparison.OrdinalIgnoreCase);
    });
});

builder.Services.AddScoped<AppDbContext>();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/Games/swagger.json", "Games API v1");
        options.SwaggerEndpoint("/swagger/Genres/swagger.json", "Genres API v1");
    });
}

app.MapGamesEndpoints();
app.MapGenresEndpoints();

app.Run();