using ChatApp.Api;
using ChatApp.Application;
using ChatApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseApiServices();

app.Run();
