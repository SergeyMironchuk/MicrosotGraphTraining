using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Mvc;
using Microsoft_Graph_SDK_ASPNET_Connect.Controllers.Http;
using Newtonsoft.Json;

namespace Microsoft_Graph_SDK_ASPNET_Connect.Controllers
{
    public class HomeController : Controller
    {
        private string _httpsLocalhost = "https://localhost:44300";
        private string _clientId = "cec5e543-f6e6-4cac-b8e2-c60ef2d346ed";
        private string _clientSecret = "nnCrOsLJeiEA2EXDR3tMFFScYAc9/KIbKTaqZ+Qk2xw=";
        private string _resourceUrl = "https://graph.microsoft.com/";
        private string _httpsLoginMicrosoftonlineCom = "https://login.microsoftonline.com";

        public ActionResult Index(string code, string command)
        {
            command = command ?? "me";
            AuthorizationToken token = Session["GraphToken"] as AuthorizationToken;

            if ((token == null || token.ExpirationDateTime <= DateTimeOffset.Now) && string.IsNullOrEmpty(code))
            {
                Request.Cookies.Clear();
                Response.Cookies.Clear();
                return Redirect($"https://login.microsoftonline.com/common/oauth2/authorize?response_type=code&redirect_uri={_httpsLocalhost}&client_id={_clientId}");
            }

            if (token == null)
            {
                token = GetAuthorizationToken(code);
                Session["GraphToken"] = token;
            }

            if (!string.IsNullOrEmpty(code)) return RedirectToAction("Index");

            var userInfo = RunGraphCommand("me", token);
            var user = JsonConvert.DeserializeObject<UserAzure>(userInfo);

            var result = RunGraphCommand(command, token);

            ViewBag.Email = JsonPrettify(result);
            ViewBag.Token = token.Token;
            ViewBag.Command = command;
            ViewBag.UserName = user.UserPrincipalName;

            return View("Graph");
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

        private AuthorizationToken GetAuthorizationToken(string code)
        {
            var requestManager = new RestRequestManager(new Uri(_httpsLoginMicrosoftonlineCom));
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", _httpsLocalhost),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
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
            return View();
        }

        public ActionResult SignOut()
        {
            Session["GraphToken"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult SignIn()
        {
            throw new NotImplementedException();
        }
    }

    public class UserAzure
    {
        public string UserPrincipalName { get; set; }
        public string DisplayName { get; set; }
    }
}