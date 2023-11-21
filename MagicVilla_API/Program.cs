using MagicVilla_API;
using MagicVilla_API.Data;
using MagicVilla_API.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/*        Serilog göstermek için eklemiþtik           */
/*Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log/villaLogs.txt",rollingInterval:RollingInterval.Day).CreateLogger();*/

// Bu bölüm, Serilog'un LoggerConfiguration sýnýfýný kullanarak loglama yapýlandýrmasýný gerçekleþtirir. Aþaðýdaki önemli özelliklere sahiptir:

//MinimumLevel.Debug(): Log seviyesini belirler. Bu durumda, Debug seviyesinden itibaren tüm loglarý kaydedecektir.

//WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day): Loglarýn dosyaya yazýlacaðý konfigürasyonu belirler. RollingInterval.Day parametresi, her gün yeni bir log dosyasý oluþturulmasýný saðlar. Log dosyalarý "log" klasörü altýnda villaLogs.txt adýyla saklanýr.

/*        Serilog göstermek için eklemiþtik           */
/*builder.Host.UseSerilog();*/

// ASP.NET Core uygulamanýzýn WebHostBuilder'ýna Serilog'u ekler. Bu, uygulamanýzýn Serilog tarafýndan yapýlandýrýlan loglama ayarlarýný kullanmasýný saðlar.

// Yukarýdaki kod bloðu, uygulamanýzýn çalýþma süresi boyunca loglama iþlemlerini Serilog ile gerçekleþtirmesini saðlar. Bu sayede, loglarý belirtilen dosyaya düzgün bir þekilde kaydedebilir ve gerektiðinde bu loglarý inceleyebilirsiniz.

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddControllers(option =>
{
    option.ReturnHttpNotAcceptable=true; 
    
    // Eðer ReturnHttpNotAcceptable özelliði true olarak ayarlanmýþsa, ve bir istemci tarafýndan kabul edilebilen bir medya türü bulunamazsa, MVC frameworkü varsayýlan olarak HTTP durum kodu 406 (Not Acceptable) ile birlikte bir hata döndürür. Bu durum, istemcinin belirli bir medya türünü kabul etmediði durumlarý ifade eder.
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
