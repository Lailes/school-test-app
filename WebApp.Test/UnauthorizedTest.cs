using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WebApp.Test
{
    public class UnauthorizedTest : MyTestBase
    {
        
        [Test(Description = "TODO 4")]
        [Repeat(30)]
        public async Task TestUnauthorized()
        {
            var result = await Client.GetAccountAsync();
            
            if (result.StatusCode == HttpStatusCode.Redirect) 
                result = await Client.GetAsync(result.Headers.Location);
            
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }
        
        [Test(Description = "TODO 5")]
        [Repeat(30)]
        public async Task TestAdminUnauthorized()
        {
            var result = await BobClient.GetAccountByIdAsync(2);
            
            if (result.StatusCode == HttpStatusCode.Redirect) 
                result = await Client.GetAsync(result.Headers.Location);
            
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}