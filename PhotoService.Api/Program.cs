
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PhotoService.Application.Interfaces;
using PhotoService.Application.Mapping;
using PhotoService.Infrastructure.Configuration;
using PhotoService.Infrastructure.DI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add Infrastructure.
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("PhotoDb") ?? throw new InvalidOperationException("Database connection not found."), 
    builder.Configuration.GetSection("RabbitMQ"),
    builder.Logging);

// Register AutoMapper
builder.Services.AddAutoMapper(p =>
{
    p.AddProfile(new PhotoMappingProfile());
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

// Authentication & Authorization
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

        };
    });
builder.Services.AddAuthorization();


var app = builder.Build();

var rabbitService = app.Services.GetRequiredService<IRabbitMqService>();
await rabbitService.InitializeAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Once Frontend React is hosted in Docker container, need to update the URL accordingly.
app.UseCors(opts => opts.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000", "https://localhost:3000"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
