using DataLayer.API.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region User Manager Identity

builder.Services.AddDbContext<UserMangerContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnectionDB")));

builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<UserMangerContext>();

#endregion


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.Run();
