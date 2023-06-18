using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySql.EntityFrameworkCore.Extensions;
using System.Text;
using techtrekAPI;
using techtrekAPI.Entities;

var builder = WebApplication.CreateBuilder(args);

// DOCU: Retrieve configuration from appsettings.json, this is where you've set your issuer and audience values for best practice.
var configuration = builder.Configuration;

// Add services to the container.

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue; // if don't set 
    //default value is: 128 MB
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// DOCU: Adding this to the dependency container allows us to inject this service to our controllers so we can perform CRUD operations.
builder.Services.AddEntityFrameworkMySQL().AddDbContext<TechtrekContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("MyConnection"));
});

// DOCU: To allow attaching of access token to requests for SwaggerUI
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// DOCU: Add a JWT Token authentication scheme.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtIssuer"],
            ValidAudience = configuration["JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JwtKey"])
            ),
        };
    });


// DOCU: to lowercase all routes in your API
builder.Services.Configure<RouteOptions>(options => {
    options.LowercaseUrls = true;
});


// DOCU: Add Cors policy
// DOCU: If you want to use this in ngRok you must expose using https not http, else it will show incomplete httprequest, will show invalid return.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// DOCU: Register the TokenService class so that it can be injected to other class via dependency injection (Adding it to the recipient class' constructor), more like what we did in the IConfiguration in TokenService.cs.
builder.Services.AddScoped<TokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// DOCU: Serving static files from ASP.Net to Frontend, access it via hosturl+relativedirectory of the image.
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Uploads")),
    RequestPath = new PathString("/Uploads")
});

app.UseHttpsRedirection();

// DOCU: use Cors and the policy declared above.
app.UseCors();

// DOCU: Add useAuthentication method.
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
