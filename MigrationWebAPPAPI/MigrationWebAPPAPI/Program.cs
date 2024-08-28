using Microsoft.OpenApi.Models;
using MigrationWebAPPAPI.CCDataServises;
using MigrationWebAPPAPI.DbInfrastructure;
using MigrationWebAPPAPI.Interfacese;
using MigrationWebAPPAPI.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MigrationApp", Version = "v1.0" });

    // Configure XML comments for your controllers and models here
    // c.IncludeXmlComments("your-xml-comments-file.xml");
});
builder.Services.Configure<MongoScoket>(builder.Configuration.GetSection("MongoObject"));
builder.Services.Configure<SQLConnectorOptions>(builder.Configuration.GetSection("SQLConnector"));
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
Log.Logger = new LoggerConfiguration()
        .WriteTo.File("C:\\Users\\ankur\\Desktop\\MigrationAPIlog\\WebApiLog\\log.txt", rollingInterval: RollingInterval.Day,
          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
           fileSizeLimitBytes: null,
                                            retainedFileCountLimit: null,
                                            shared: true,
                                            flushToDiskInterval: TimeSpan.FromSeconds(5))

        .CreateLogger();
builder.Logging.AddSerilog();

builder.Services.AddMvc().AddXmlSerializerFormatters();


builder.Services.AddMvc().AddXmlSerializerFormatters();

builder.Services.AddScoped<ISQLConnector, SQLRepository>();
builder.Services.AddScoped<ICOSMOSConnector,CosmosRepository>();
builder.Services.AddScoped<IDataServises, CCDataGetServises>();
builder.Services.AddScoped<SQLRepository>();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));



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
