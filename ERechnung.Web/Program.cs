using ERechnung.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddScoped<InvoiceRecalculationService>();
builder.Services.AddScoped<InvoiceBusinessValidator>();
builder.Services.AddScoped<XRechnungGenerator>();
builder.Services.AddScoped<IInvoiceImportService, InvoiceImportService>();

builder.Services.AddRazorPages();

var app = builder.Build();
// app.UsePathBase("/xrechnung");
// 🔴 NUR IN PRODUKTION
if (!app.Environment.IsDevelopment())
{
    app.UsePathBase("/xrechnung");
}

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

//using ERechnung.Core.Services;
//using PdfSharp.Charting;

//var builder = WebApplication.CreateBuilder(args);

//// DI
//builder.Services.AddScoped<InvoiceRecalculationService>();
//builder.Services.AddScoped<InvoiceBusinessValidator>();
//builder.Services.AddScoped<XRechnungGenerator>();
//builder.Services.AddScoped<IInvoiceImportService, InvoiceImportService>();


//// Add services to the container.
//builder.Services.AddRazorPages();

//var app = builder.Build();

//// 🔴 Basis - Pfad setzen
//app.UsePathBase("/xrechnung");

//// Optional: RequestPath normalisieren
//app.Use(async (context, next) =>
//{
//    context.Request.PathBase = "/xrechnung";
//    await next();
//});

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();

//app.UseRouting();

//app.UseAuthorization();

//app.MapStaticAssets();
//app.MapRazorPages()
//   .WithStaticAssets();

//app.Run();
