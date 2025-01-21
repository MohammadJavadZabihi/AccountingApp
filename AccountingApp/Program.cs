using Core.API.Service;
using Core.API.Service.Interface;
using DataLayer.API.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region User Manager Identity

builder.Services.AddDbContext<UserMangerContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnectionDB")));

builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<UserMangerContext>();

#endregion

#region Api Version Config

builder.Services.AddApiVersioning(option =>
{
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.ReportApiVersions = true;
    option.ApiVersionReader = ApiVersionReader.Combine
    (
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("x-api-version"),
        new UrlSegmentApiVersionReader()
    );
});

#endregion

#region Add Rate Limiter

builder.Services.AddRateLimiter(options =>
{
    #region LoginRegisterPolicy
    /*
    AddPolicy for user who wants to register or login (without jwt token)
    Set 6 api per min and 1 QueueLimit
    Get partitionKey => RemoteIpAddress :)
    */

    options.AddPolicy("LoginRegisterPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "Login Anonymous",
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 6,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 1,
            }));

    #endregion

    #region AuthenticatePolicy

    /*AddPolicy for user who logining in account and wnats send api :)
     Set 10 api per sec and 3 QueueLimiti
     Get partitionKey => from Identity.Name
     */

    options.AddPolicy("AuthenticatePolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity.Name ?? "unknow",
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 8,
                Window = TimeSpan.FromSeconds(10),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 3
            }));

    #endregion

    #region Add Custom Error

    options.OnRejected = async (context, CancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;

        await context.HttpContext.Response.WriteAsync("You have reach the maximum number of sending api, please wait and try again later");
    };

    #endregion
});

#endregion

#region Add JWT Configuration

var jwtSetting = builder.Configuration.GetSection("jwt");
var key = Encoding.ASCII.GetBytes(jwtSetting["key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSetting["Issuer"],
        ValidAudience = jwtSetting["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
    };
});

#endregion

#region IOC (InVersion Of Controlle)

builder.Services.AddTransient<IUserServiceAsync, UserServiceAsync>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseRateLimiter();


app.Run();
