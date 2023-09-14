using boardgame.Drivers;
using SignalrClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<SignalRDriver>();
builder.Services.AddSingleton<ISignalRService, SignalRService>();
//builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();


app.Run();
