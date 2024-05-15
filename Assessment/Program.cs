using Assessment.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Assessment.Controllers;
using Assessment.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AssessmentContext>(
options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("productvideodb"),
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.4.0-mysql"));
});


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

app.MapProductEndpoints();

app.Run();
