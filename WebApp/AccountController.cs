using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp
{
    // TODO_ 4: unauthorized users should receive 401 status code
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize] 
        [HttpGet]
        public async ValueTask<ActionResult<Account>> Get()
        {
            var userId = User.Claims
                .Where(claim => claim.Type == ClaimTypes.Name)
                .Select(claim => claim.Value)
                .FirstOrDefault();

            if (userId == null)
                return NotFound();

            var account = await _accountService.LoadOrCreateAsync(userId /* TODO_ 3: Get user id from cookie */);
            return new ActionResult<Account>(account);
        }

        //TODO_ 5: Endpoint should works only for users with "Admin" Role
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public ValueTask<Account> GetByInternalId([FromRoute] int id)
        {
            return _accountService.GetFromCache(id);
        }

        [Authorize]
        [HttpPost("counter")]
        public async Task UpdateAccount()
        {
            //Update account in cache, don't bother saving to DB, this is not an objective of this task.
            var actionResult = await Get();
            if (actionResult.Result != NotFound())
            {
                var account = actionResult.Value;
                account.Counter++;
            }
        }
    }
}