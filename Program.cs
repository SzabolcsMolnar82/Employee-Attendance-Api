using Employee_Attendance_Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Render.com 8080 port fix
builder.WebHost.UseUrls("http://0.0.0.0:8080");

//SQL szerver lehetõség
/*
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
*/
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//Ez kell az Authorization gombhoz 2025.03.07
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Bearer {eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluMiIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTc0MTM2MTczNCwiZXhwIjoxNzQxMzY1MzM0LCJpYXQiOjE3NDEzNjE3MzR9.M6B4AWWw695LmFKXL-25xqMVS2OloJ_dVRS2bY533Nw}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});






builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {

        //Teszt ahhoz hogy megfelelõ e a hash!
        /*
        var secretKey = "8[=5WQ#vDCA[g@p$YyFVYXEqm7*x)mS6";
        Console.WriteLine($"Secret Key Length: {secretKey.Length}"); // Ellenõrizd, hogy 32 karakter hosszú-e!
        Console.WriteLine($"Secret Key Base64: {Convert.ToBase64String(Encoding.UTF8.GetBytes(secretKey))}");
        */

        var secretKey = builder.Configuration["JwtSettings:Secret"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("8[=5WQ#vDCA[g@p$YyFVYXEqm7*x)mS6"))
        };
    });
builder.Services.AddAuthorization();

//ez kell majd ahhoz hogy menjen a fetchelés a Svelte felé, utána még lentebb is van egy sor!
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});




var app = builder.Build();

/*
if (app.Environment.IsDevelopment())
{
   ;
}
*/

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();