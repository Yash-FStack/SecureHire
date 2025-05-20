using CheatingProofInterviewSystem.Authentication;
using CheatingProofInterviewSystem.Services;
using Microsoft.AspNetCore.Authentication; // Match your namespace

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Add dummy authentication to support [Authorize] without Identity
builder.Services.AddAuthentication("DummyScheme")
    .AddScheme<AuthenticationSchemeOptions, DummyAuthHandler>("DummyScheme", null);

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();    // ✅ Required before UseAuthorization
app.UseAuthorization();     // ✅ Enables [Authorize]

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
