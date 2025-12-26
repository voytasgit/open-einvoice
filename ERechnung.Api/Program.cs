using ERechnung.Core.Exceptions;
using ERechnung.Core.Services;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddScoped<InvoiceRecalculationService>();
builder.Services.AddScoped<InvoiceBusinessValidator>();
builder.Services.AddScoped<XRechnungGenerator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseExceptionHandler(handler =>
{
    handler.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is BusinessException)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new
            {
                error = exception.Message
            });
        }
        else
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Internal server error"
            });
        }
    });
});


app.Run();
