using System.Threading.Tasks;
using NUnit.Framework;

namespace WebApp.Test;

public class AccountControllerTests : MyTestBase
{
    [Test (Description = "TODO 3 Test")]
    [TestCase("alice@mailinator.com", 1)]
    public async Task AccountControllerGetTest(string userName, int userIdExpected)
    {
        using var client = await CreateAuthorizedClientAsync(userName);
        using var response = await client.GetAccountAsync();
        var account = await response.Response<Account>();
        
        Assert.AreEqual(userName, account.UserName);
        Assert.AreEqual(userIdExpected, account.InternalId);
    }
}