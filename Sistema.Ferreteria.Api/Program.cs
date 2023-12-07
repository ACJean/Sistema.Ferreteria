using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sistema.Ferreteria.Core.Articulo.Aplicacion;
using Sistema.Ferreteria.Core.Articulo.Dominio;
using Sistema.Ferreteria.Core.Articulo.Infraestructura;
using Sistema.Ferreteria.Core.Cliente.Aplicacion;
using Sistema.Ferreteria.Core.Cliente.Dominio;
using Sistema.Ferreteria.Core.Cliente.Infraestructura;
using Sistema.Ferreteria.Core.Seguridad.Aplicacion;
using Sistema.Ferreteria.Core.Seguridad.Dominio;
using Sistema.Ferreteria.Core.Seguridad.Infraestructura;
using Sistema.Ferreteria.Core.Venta.Aplicacion;
using Sistema.Ferreteria.Core.Venta.Dominio;
using Sistema.Ferreteria.Core.Venta.Infraestructura;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios, en este caso, CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
         ClockSkew = TimeSpan.Zero
     };
 });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    // Bearer token authentication
    OpenApiSecurityScheme securityDefinition = new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        BearerFormat = "JWT",
        Scheme = "Bearer",
        Description = "Specify the authorization token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    };
    setup.AddSecurityDefinition("Bearer", securityDefinition);

    OpenApiSecurityRequirement securityRequirement = new OpenApiSecurityRequirement();
    OpenApiSecurityScheme secondSecurityDefinition = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    securityRequirement.Add(secondSecurityDefinition, new string[] { });
    setup.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddScoped<IUsuarioRepository, PgsqlUsuarioRepository>();
builder.Services.AddScoped<IArticuloRepository, PgsqlArticuloRepository>();
builder.Services.AddScoped<IClienteRepository, PgsqlClienteRepository>();
builder.Services.AddScoped<ICuentaRepository, PgsqlCuentaRepository>();

builder.Services.AddScoped<SeguridadManager>();
builder.Services.AddScoped<ArticulosManager>();
builder.Services.AddScoped<ClienteManager>();
builder.Services.AddScoped<CuentaManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowSpecificOrigin");

app.MapControllers();

app.Run();
