using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using System.Net.Http.Headers;

namespace ThAmCo.Accounts.Api.Services
{
    public class AccountService : IAccountsService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public AccountService(IHttpClientFactory clientFactory, 
                              IConfiguration configuration 
                              )
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        record TokenDto(string access_token, string token_type, int expires_in);

        //get all accounts from auth0 management api
        public async Task<IEnumerable<AccountDto>> GetAccountsAsync()
        {

            //create token client
            var tokenClient = _clientFactory.CreateClient();

            //assign authority
            var authBaseAddress = _configuration["Auth:Authority"];
            tokenClient.BaseAddress = new Uri(authBaseAddress);
            //create token parameters
            var tokenParams = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _configuration["Auth:ClientId"] },
                { "client_secret", _configuration["Auth:ClientSecret"] },
                { "audience", _configuration["WebServices:AccountsAPI:AuthAudience"] },
            };

            var tokenFrom = new FormUrlEncodedContent(tokenParams);
            //post token params to endpoint
            var tokenResponse = await tokenClient.PostAsync("oauth/token", tokenFrom);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenInfo = await tokenResponse.Content.ReadFromJsonAsync<TokenDto>();

            //create client
            var client = _clientFactory.CreateClient();

            //configure client
            var serviceBaseAddress = _configuration["WebServices:AccountsAPI:BaseURL"];
            client.BaseAddress = new Uri(serviceBaseAddress);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenInfo?.access_token);

            // http get to serivceBaseAddress/users
            HttpResponseMessage response = await client.GetAsync("users");
            //response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<IEnumerable<AccountDto>>();
            return result;

        }

        //get account via id from auth0 management api
        public async Task<AccountDto> GetAccountAsync(string? id)
        {
            var tokenClient = _clientFactory.CreateClient();

            var authBaseAddress = _configuration["Auth:Authority"];
            tokenClient.BaseAddress = new Uri(authBaseAddress);

            var tokenParams = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _configuration["Auth:ClientId"] },
                { "client_secret", _configuration["Auth:ClientSecret"] },
                { "audience", _configuration["WebServices:AccountsAPI:AuthAudience"] },
            };

            var tokenFrom = new FormUrlEncodedContent(tokenParams);
            var tokenResponse = await tokenClient.PostAsync("oauth/token", tokenFrom);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenInfo = await tokenResponse.Content.ReadFromJsonAsync<TokenDto>();

            var client = _clientFactory.CreateClient();

            var serviceBaseAddress = _configuration["WebServices:AccountsAPI:BaseURL"];
            client.BaseAddress = new Uri(serviceBaseAddress);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenInfo?.access_token);

            HttpResponseMessage response = await client.GetAsync("users/" + id);
            //response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<AccountDto>();
            return result;

        }

        //create account via auth0 management api
        public async Task<AccountsCreationViewModel> CreateAccountAsync(AccountsCreationViewModel account)
        {
            var tokenClient = _clientFactory.CreateClient();

            var authBaseAddress = _configuration["Auth:Authority"];
            tokenClient.BaseAddress = new Uri(authBaseAddress);

            var tokenParams = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _configuration["Auth:ClientId"] },
                { "client_secret", _configuration["Auth:ClientSecret"] },
                { "audience", _configuration["WebServices:AccountsAPI:AuthAudience"] },
            };

            var tokenFrom = new FormUrlEncodedContent(tokenParams);
            var tokenResponse = await tokenClient.PostAsync("oauth/token", tokenFrom);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenInfo = await tokenResponse.Content.ReadFromJsonAsync<TokenDto>(); ;

            var client = _clientFactory.CreateClient();

            var AccountParams = new Dictionary<string, string>
            {
                {"user_id", account.user_id  },
                {"username", account.username},
                {"name", account.name },
                {"nickname", account.nickname },
                {"email", account.email},
                {"password", account.password},
                {"connection", "Username-Password-Authentication"}

            };

            var serviceBaseAddress = _configuration["WebServices:AccountsAPI:BaseURL"];
            client.BaseAddress = new Uri(serviceBaseAddress);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenInfo?.access_token);

            HttpResponseMessage response = await client.PostAsJsonAsync("users",AccountParams);
            //response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<AccountsCreationViewModel>();
            return result;
        }

        //delete account via aith0 management api
        public async Task<ActionResult<bool>> DeleteAccountAsync(string? id)
        {
            var tokenClient = _clientFactory.CreateClient();

            var authBaseAddress = _configuration["Auth:Authority"];
            tokenClient.BaseAddress = new Uri(authBaseAddress);

            var tokenParams = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _configuration["Auth:ClientId"] },
                { "client_secret", _configuration["Auth:ClientSecret"] },
                { "audience", _configuration["WebServices:AccountsAPI:AuthAudience"] },
            };

            var tokenFrom = new FormUrlEncodedContent(tokenParams);
            var tokenResponse = await tokenClient.PostAsync("oauth/token", tokenFrom);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenInfo = await tokenResponse.Content.ReadFromJsonAsync<TokenDto>();

            var client = _clientFactory.CreateClient();

            var serviceBaseAddress = _configuration["WebServices:AccountsAPI:BaseURL"];
            client.BaseAddress = new Uri(serviceBaseAddress);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenInfo?.access_token);

            HttpResponseMessage response = await client.DeleteAsync("users/" + id);
            //response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<AccountDto>();
            return true;

        }


        //edit account via auth0 management api
        public async Task<AccountsCreationViewModel> EditAccountAsync(AccountsCreationViewModel account, string id)
        {
            var tokenClient = _clientFactory.CreateClient();

            var authBaseAddress = _configuration["Auth:Authority"];
            tokenClient.BaseAddress = new Uri(authBaseAddress);

            var tokenParams = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _configuration["Auth:ClientId"] },
                { "client_secret", _configuration["Auth:ClientSecret"] },
                { "audience", _configuration["WebServices:AccountsAPI:AuthAudience"] },
            };

            var tokenFrom = new FormUrlEncodedContent(tokenParams);
            var tokenResponse = await tokenClient.PostAsync("oauth/token", tokenFrom);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenInfo = await tokenResponse.Content.ReadFromJsonAsync<TokenDto>(); ;

            var client = _clientFactory.CreateClient();

            var AccountParams = new Dictionary<string, string>
            {
                {"user_id", account.user_id  },
                {"username", account.username},
                {"name", account.name },
                {"nickname", account.nickname },
                {"email", account.email},
                {"password", account.password},
                {"connection", "Username-Password-Authentication"}

            };

            var serviceBaseAddress = _configuration["WebServices:AccountsAPI:BaseURL"];
            client.BaseAddress = new Uri(serviceBaseAddress);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenInfo?.access_token);

            HttpResponseMessage response = await client.PutAsJsonAsync("users", AccountParams);
            //response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<AccountsCreationViewModel>();
            return result;
        }




    }
}
