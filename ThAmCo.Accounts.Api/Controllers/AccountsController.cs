using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThAmCo.Accounts.Api.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ThAmCo.Accounts.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IAccountsService _accountsService;

        public AccountsController(IAccountsService accountsService)
        {
            
            _accountsService = accountsService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<AccountDto>> Index()
        {
            
            IEnumerable<AccountDto> accounts = null;
            try
            {
                accounts = await _accountsService.GetAccountsAsync();
            }
            catch
            {
                accounts = Array.Empty<AccountDto>();
            }

            return (accounts);
        }

        // GET api/<AccountsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> Get(string id)
        {
            AccountDto account = null;
            try
            {
                account = await _accountsService.GetAccountAsync(id);
            }
            catch
            {
                account = null;
            }
            return account;
        }



        // POST api/<AccountsController>
        [HttpPost]
        public async Task <ActionResult> Create(AccountsCreationViewModel account)
        {
            try
            {
                account = await _accountsService.CreateAccountAsync(account);
            }
            catch
            {
                _logger.LogWarning("Exception occured using the Accounts Service");
            }
            

            return View(account);
        }

        // PUT api/<AccountsController>/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> Edit(AccountsCreationViewModel account, string id)
        {
            try
            {
                account = await _accountsService.EditAccountAsync(account, id);
            }
            catch
            {
                _logger.LogWarning("Exception occured using the Accounts Service");
            }
            return View();

        }

        // DELETE api/<AccountsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            bool deleted = false;
            try
            {
                await _accountsService.DeleteAccountAsync(id);
                deleted = true;
            }
            catch
            {
                deleted = false;
            }
            return View();
        }
    }
}
