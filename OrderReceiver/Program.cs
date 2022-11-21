using OrderReceiver.Common.Deserialization;
using OrderReceiver.Helpers;
using OrderReceiver.InputFormatters;
using OrderReceiver.Testing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Add(new CsvStringInputFormatter());
    options.InputFormatters.Add(new XmlStringInputFormatter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TestDbContext>();
builder.Services.AddSingleton<IOrderDeserializer, OrderDeserializer>();
builder.Services.AddScoped<IOrderProcessor, DbOrderProcessor>((c) => new DbOrderProcessor(dbContext: c.GetRequiredService<TestDbContext>()));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
