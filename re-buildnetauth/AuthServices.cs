using Microsoft.AspNetCore.DataProtection;

public class AuthServices {
    private readonly IDataProtectionProvider idp;
    private readonly IHttpContextAccessor accessor;

    public AuthServices(IDataProtectionProvider _idp, IHttpContextAccessor _accessor)
    {
        idp = _idp;
        accessor = _accessor;
    }



    public void SignIn() {
        var protector = idp.CreateProtector("auth-cookie");
        accessor.HttpContext.Response.Headers["set-cookie"] = $"auth= {protector.Protect("usr:paul")}";
     }

}
