using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using CatalogoZap.Infrastructure.JWT;
using CatalogoZap.Infrastructure.Swagger;
using CatalogoZap.Services;
using CatalogoZap.Repositories;
using System.Data;
using Npgsql;
using CatalogoZap.Infrastructure.CloudinaryService;
using CatalogoZap.Infrastructure.SendGrid;
using CatalogoZap.Infrastructure.Exceptions;
using CatalogoZap.Infrastructure.Handlers;
using CatalogoZap.Options.Cloudinary;
using CatalogoZap.Options.Token;
using CatalogoZap.Options.SendGrid;
using CatalogoZap.Options.FrontEnd;
using DotNetEnv;
using Resend;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

if (builder.Environment.IsDevelopment()) {
    var connectionString = builder.Configuration["CONNECTION_STRING"];

    builder.Configuration.GetSection("ConnectionStrings")["Default"] = connectionString;
}

var cloudinaryKey = builder.Configuration["CLOUDINARY_KEY"];

if (string.IsNullOrEmpty(cloudinaryKey))
{
    throw new InvalidOperationException("Can not find CLOUDINARY_KEY");
}

builder.Services.Configure<CloudinaryOptions>(options =>
{
    options.CloudinaryKey = cloudinaryKey;
});

var frontEndUrl = builder.Configuration["FRONTEND_URL"];

if (string.IsNullOrEmpty(frontEndUrl))
{
    throw new InvalidOperationException("Can not find FRONTEND_URL");
}

builder.Services.Configure<FrontEndOptions>(options =>
{
    options.FrontEndUrl = frontEndUrl;
});

var JwtKey = builder.Configuration["JWT_KEY"];
var JwtIssuer = builder.Configuration["JWT_ISSUER"];
var JwtAudience = builder.Configuration["JWT_AUDIENCE"];

if (string.IsNullOrEmpty(JwtKey) || string.IsNullOrEmpty(JwtIssuer) || string.IsNullOrEmpty(JwtAudience))
{
    throw new InvalidOperationException("Can not find JWT_KEY or JWT_ISSUER or JWT_AUDIENCE");
}

builder.Services.Configure<TokenOptions>(options =>
{
    options.JwtKey = JwtKey;
	options.JwtIssuer = JwtIssuer;
	options.JwtAudience = JwtAudience;
});

var SendGridApiKey = builder.Configuration["SENDGRID_APIKEY"];
var SendGridEmail = builder.Configuration["SENDGRID_EMAIL"];

if (string.IsNullOrEmpty(SendGridApiKey) || string.IsNullOrEmpty(SendGridEmail))
{
    throw new InvalidOperationException("Can not find SENDGRID_EMAIL or SENDGRID_EMAIL");
}

builder.Services.Configure<SendGridOptions>(options =>
{
    options.SendGridApiKey = SendGridApiKey;
	options.SendGridEmail = SendGridEmail;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(options => {
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = TokenService.GetValidationParameters(JwtKey!, JwtIssuer!, JwtAudience!);
});

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c => {
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header
	});

	c.OperationFilter<AuthorizeCheckOperationFilter>();
	c.OperationFilter<AutoTagOperationFilter>();
	c.EnableAnnotations();
});

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<SendGridService>();
builder.Services.AddScoped<ProductsService>();
builder.Services.AddScoped<ProductsRepository>();
builder.Services.AddScoped<ProfilesService>();
builder.Services.AddScoped<ProfilesRepository>();
builder.Services.AddScoped<StoresService>();
builder.Services.AddScoped<StoresRepository>();

builder.Services.AddScoped<IDbConnection>(sp =>
	new NpgsqlConnection(builder.Configuration["CONNECTION_STRING"])
);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
