using MagicVilla_API;
using MagicVilla_API.Data;
using MagicVilla_API.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/*        Serilog g�stermek i�in eklemi�tik           */
/*Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log/villaLogs.txt",rollingInterval:RollingInterval.Day).CreateLogger();*/

// Bu b�l�m, Serilog'un LoggerConfiguration s�n�f�n� kullanarak loglama yap�land�rmas�n� ger�ekle�tirir. A�a��daki �nemli �zelliklere sahiptir:

//MinimumLevel.Debug(): Log seviyesini belirler. Bu durumda, Debug seviyesinden itibaren t�m loglar� kaydedecektir.

//WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day): Loglar�n dosyaya yaz�laca�� konfig�rasyonu belirler. RollingInterval.Day parametresi, her g�n yeni bir log dosyas� olu�turulmas�n� sa�lar. Log dosyalar� "log" klas�r� alt�nda villaLogs.txt ad�yla saklan�r.

/*        Serilog g�stermek i�in eklemi�tik           */
/*builder.Host.UseSerilog();*/

// ASP.NET Core uygulaman�z�n WebHostBuilder'�na Serilog'u ekler. Bu, uygulaman�z�n Serilog taraf�ndan yap�land�r�lan loglama ayarlar�n� kullanmas�n� sa�lar.

// Yukar�daki kod blo�u, uygulaman�z�n �al��ma s�resi boyunca loglama i�lemlerini Serilog ile ger�ekle�tirmesini sa�lar. Bu sayede, loglar� belirtilen dosyaya d�zg�n bir �ekilde kaydedebilir ve gerekti�inde bu loglar� inceleyebilirsiniz.

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddControllers(option =>
{
    option.ReturnHttpNotAcceptable=true; 
    
    // E�er ReturnHttpNotAcceptable �zelli�i true olarak ayarlanm��sa, ve bir istemci taraf�ndan kabul edilebilen bir medya t�r� bulunamazsa, MVC framework� varsay�lan olarak HTTP durum kodu 406 (Not Acceptable) ile birlikte bir hata d�nd�r�r. Bu durum, istemcinin belirli bir medya t�r�n� kabul etmedi�i durumlar� ifade eder.
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
