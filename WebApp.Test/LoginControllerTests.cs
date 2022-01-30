using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WebApp.Test;

public class LoginControllerTests : MyTestBase
{
    public async void TestAuthCookie()
    {
        
    }
    
    [Test]
    [TestCase("not_existing@mailinator.com", HttpStatusCode.NotFound)]
    [TestCase("alice@mailinator.com", HttpStatusCode.OK)]
    [TestCase("bob@mailinator.com", HttpStatusCode.OK)]
    public async Task TestUserNotFound(string sighInName, HttpStatusCode code)
    {
        using var result = await Client.SignInAsync(sighInName);
        Assert.AreEqual(code, result.StatusCode);
    }
}