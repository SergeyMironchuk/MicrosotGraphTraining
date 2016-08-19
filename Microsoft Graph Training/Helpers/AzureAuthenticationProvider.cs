using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft_Graph_SDK_ASPNET_Connect.Helpers
{
    class AzureAuthenticationProvider : IAuthenticationProvider
    {
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            string clientId = "8c0c6071-c66b-4d68-86ad-b2004408fae1";
            string clientSecret = "3DAGnhno6tcF350HyeTEMBc8M9H2larjNhFHkNOSoGs=";

            //AuthenticationContext authContext = new AuthenticationContext("https://login.windows.net/mironchuk.onmicrosoft.com/oauth2/token");
            AuthenticationContext authContext = new AuthenticationContext("https://login.microsoftonline.com/c35920e8-36cf-49ea-ae35-c1d742ec15d3/oauth2/token");

            ClientCredential creds = new ClientCredential(clientId, clientSecret);

            AuthenticationResult authResult = await authContext.AcquireTokenAsync("https://graph.microsoft.com/", creds);

            request.Headers.Add("Authorization", "Bearer " + authResult.AccessToken);
        }
    }
}