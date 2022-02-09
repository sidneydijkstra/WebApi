var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ApiConfigExtensions.AddConfig(builder.Services, builder.Configuration);
ApiDatabaseExtensions.AddConfig(builder.Services, builder.Configuration);
ApiAuthenticationExtensions.AddConfig(builder.Services, builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ApiSwaggerExtensions.AddConfig(builder.Services);

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

app.Run();
