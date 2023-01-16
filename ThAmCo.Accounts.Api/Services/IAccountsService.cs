using Microsoft.AspNetCore.Mvc;

namespace ThAmCo.Accounts.Api.Services
{
    public interface IAccountsService
    {
        Task <IEnumerable<AccountDto>> GetAccountsAsync();

        Task<AccountDto> GetAccountAsync(string accountId);

        Task<AccountsCreationViewModel> CreateAccountAsync(AccountsCreationViewModel account);

        Task<ActionResult<bool>> DeleteAccountAsync(string id);

        Task<AccountsCreationViewModel> EditAccountAsync(AccountsCreationViewModel account, string id);
    }
}
