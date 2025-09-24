using PhotoService.Application.Interfaces;
using PhotoService.Application.Mapping;
using PhotoService.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

// Add Infrastructure.
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("PhotoDb") ?? throw new InvalidOperationException("Database connection not found."), builder.Logging);

// Register AutoMapper
builder.Services.AddAutoMapper(p =>
{
    p.AddProfile(new PhotoMappingProfile());
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opts => opts.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000", "https://localhost:3000"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
