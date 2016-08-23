using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Mvc;
using GraphTraining.HttpServices;
using Newtonsoft.Json;

namespace GraphTraining.WebUI.Controllers
{
    public class HomeController : Controller
    {
        // sergey@smironchuk.onmicrosoft.com
        //private string _clientId = "cec5e543-f6e6-4cac-b8e2-c60ef2d346ed";
        //private string _clientSecret = "nnCrOsLJeiEA2EXDR3tMFFScYAc9/KIbKTaqZ+Qk2xw=";
        // sergey@sergeym.onmicrosoft.com
        private string _clientId = "ea1e491a-a089-4698-8051-3a1b9aab1a8d";
        private string _clientSecret = "lSvgxCbFzeoOxbkLPwE+V1dtR1rk0rB4Vy7lKbVbunU=";

        private string _httpsLocalhost = "https://localhost:44300/home/login";
        private string _resourceUrl = "https://graph.microsoft.com/";
        private string _httpsLoginMicrosoftonlineCom = "https://login.microsoftonline.com";

        public ActionResult Index(string command)
        {
            var clientId = Session["_clientId"] as string ?? _clientId;
            var clientSecret = Session["_clientSecret"] as string ?? _clientSecret;
            Session["_clientId"] = clientId;
            Session["_clientSecret"] = clientSecret;

            command = command ?? "me";
            AuthorizationToken token = Session["GraphToken"] as AuthorizationToken;

            if (token != null)
            {
                var userInfo = RunGraphCommand("me", token);
                var user = JsonConvert.DeserializeObject<UserAzure>(userInfo);

                var result = RunGraphCommand(command, token);

                ViewBag.CommandResult = JsonPrettify(result);
                ViewBag.Token = token.Token;
                ViewBag.UserName = user.UserPrincipalName;
            }

            ViewBag.Command = command;
            ViewBag.ClientId = Session["_clientId"];
            ViewBag.ClientSecret = Session["_clientSecret"];
            return View();
        }

        private string RunGraphCommand(string command, AuthorizationToken token)
        {
            var headers = new HeaderParameters
            {
                { "Authorization", $"Bearer {token.Token}"},
                {"Accept-Charset", "UTF-8" }
            };
            var requestManager = new RestRequestManager(new Uri(_resourceUrl));
            var result = requestManager.Get(
                $"v1.0/{command.TrimStart('/')}",
                HttpRequestContentTypes.Application.Json,
                headers);
            return result;
        }

        public ActionResult Login(string code, string clientId, string clientSecret)
        {
            clientId = clientId ?? (Session["_clientId"] as string ?? _clientId);
            clientSecret = clientSecret ?? (Session["_clientSecret"] as string ?? _clientSecret);
            Session["_clientId"] = clientId;
            Session["_clientSecret"] = clientSecret;

            AuthorizationToken token = Session["GraphToken"] as AuthorizationToken;

            if (string.IsNullOrEmpty(code))
            {
                Request.Cookies.Clear();
                Response.Cookies.Clear();
                return Redirect($"https://login.microsoftonline.com/common/oauth2/authorize?response_type=code&redirect_uri={_httpsLocalhost}&client_id={clientId}");
            }

            if (token == null)
            {
                token = GetAuthorizationToken(code, clientId, clientSecret);
                Session["GraphToken"] = token;
            }

            return RedirectToAction("Index");
        }

        private AuthorizationToken GetAuthorizationToken(string code, string clientId, string clientSecret)
        {
            var requestManager = new RestRequestManager(new Uri(_httpsLoginMicrosoftonlineCom));
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", _httpsLocalhost),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("resource", _resourceUrl),
            });

            var token = requestManager.Post<AuthorizationToken>(
                "common/oauth2/token",
                "application/json",
                new HeaderParameters(),
                content);

            return token;
        }

        private static string JsonPrettify(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class UserAzure
    {
        public string UserPrincipalName { get; set; }
        public string DisplayName { get; set; }
    }
}