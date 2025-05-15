using TagEditor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRouting()
    .AddMvc();

builder.Services
    .AddScoped<ChatCompletionService>()
    .AddSingleton<Storage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

var port = Environment.GetEnvironmentVariable("PORT");
string? url = null;
if (port != null)
{
    url = $"http://0.0.0.0:{port}";
}
app.Run(url);
