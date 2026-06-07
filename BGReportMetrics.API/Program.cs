using BGReportMetrics.API.Data;
using BGReportMetrics.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── Two separate database contexts ───────────────────────────────────────
builder.Services.AddDbContext<MglDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("MglConnection")));

builder.Services.AddDbContext<LimsDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("LimsConnection")));

builder.Services.AddScoped<MetricsService>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ── Recreate & seed both databases ──────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var mgl  = scope.ServiceProvider.GetRequiredService<MglDbContext>();
    var lims = scope.ServiceProvider.GetRequiredService<LimsDbContext>();

    mgl.Database.EnsureDeleted();
    mgl.Database.EnsureCreated();
    await DbInitializer.SeedMglAsync(mgl);

    lims.Database.EnsureDeleted();
    lims.Database.EnsureCreated();
    await DbInitializer.SeedLimsAsync(lims);
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

app.Run();
