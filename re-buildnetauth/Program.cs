using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDataProtection();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<AuthServices>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
// app.MapGet("/username", (HttpContext ctx) => {
//     ctx.Request.Headers.Cookie.FirstOrDefault()
// });

app.Use((ctx, next) => 
{
    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
    var protector = idp.CreateProtector("auth-cookie");
    var authCookies = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var protectpayload = authCookies.Split("=").Last();
    var payload = protector.Unprotect(protectpayload);
    var part = payload.Split(":");
    var key = part[0];
    var value = part[1];

    var claims = new List<Claim>();
    claims.Add(new Claim(key,value));
    var idenity = new ClaimsIdentity(claims);
    ctx.User = new ClaimsPrincipal(idenity);
    return next();
});

app.MapGet("/username", (HttpContext ctx) => 
{
    return ctx.User.FindFirst("usr").Value;
    //return "paul";
});

app.MapGet("/login",  (AuthServices auth) => 
{
    auth.SignIn();
    return "ok";
});


app.Run();
