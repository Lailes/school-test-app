using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;

namespace WebApp.Test;

public class LoginControllerTests : MyTestBase
{
    
    [Test (Description = "TODO 1 Test")]
    [TestCase("alice@mailinator.com")]
    public async Task TestAuthCookie(string userName)
    {
        var result = await Client.SignInAsync(userName);
        result.EnsureSuccessStatusCode();
        
        result.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);

        Assert.True(cookies != null && cookies.All(cookie => cookie.StartsWith(".AspNetCore.Cookies=")));
    }
    
    [Test (Description = "TODO 2 Test")]
    [TestCase("not_existing@mailinator.com", HttpStatusCode.NotFound)]
    [TestCase("alice@mailinator.com", HttpStatusCode.OK)]
    [TestCase("bob@mailinator.com", HttpStatusCode.OK)]
    public async Task TestUserNotFound(string sighInName, HttpStatusCode code)
    {
        using var result = await Client.SignInAsync(sighInName);
        Assert.AreEqual(code, result.StatusCode);
    }
}