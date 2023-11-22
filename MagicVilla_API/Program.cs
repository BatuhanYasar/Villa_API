using MagicVilla_API;
using MagicVilla_API.Data;
using MagicVilla_API.Logging;
using MagicVilla_API.Repository;
using MagicVilla_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddControllers(option =>
{
    option.ReturnHttpNotAcceptable=false; 
    
    // E�er ReturnHttpNotAcceptable �zelli�i true olarak ayarlanm��sa, ve bir istemci taraf�ndan kabul edilebilen bir medya t�r� bulunamazsa, MVC framework� varsay�lan olarak HTTP durum kodu 406 (Not Acceptable) ile birlikte bir hata d�nd�r�r. Bu durum, istemcinin belirli bir medya t�r�n� kabul etmedi�i durumlar� ifade eder.
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//IVillaRepository, VillaRepository
builder.Services.AddScoped<IVillaRepository, VillaRepository>();

//ILogging, Logging
builder.Services.AddSingleton<ILogging, Logging>();
//ILogging, LoggingV2
builder.Services.AddSingleton<ILogging, LoggingV2>();

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
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

app.Run();
