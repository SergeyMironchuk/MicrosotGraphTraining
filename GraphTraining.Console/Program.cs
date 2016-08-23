using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using GraphTraining.HttpServices;
using Newtonsoft.Json;

namespace GraphTraining.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string accountId = "cefd786b-5520-4fdc-bc85-764cee59833c";
            string clientId = "ea1e491a-a089-4698-8051-3a1b9aab1a8d";
            string clientSecret = "lSvgxCbFzeoOxbkLPwE+V1dtR1rk0rB4Vy7lKbVbunU=";
            string resourceUrl = "https://graph.microsoft.com";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("resource", resourceUrl)
            });

            var requestManager = new RestRequestManager(new Uri("https://login.microsoftonline.com"));
            var headers = new HeaderParameters();

            //var token = requestManager.Post<AuthorizationToken>(
            //    $"{accountId}/oauth2/token",
            //    HttpRequestContentTypes.Application.UrlEncoded,
            //    headers,
            //    content);

            //System.Console.WriteLine("{0}", token.ExpirationDateTime);
            //System.Console.WriteLine("{0}", token.Token);

            var Token =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJodHRwczovL2dyYXBoLm1pY3Jvc29mdC5jb20vIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvY2VmZDc4NmItNTUyMC00ZmRjLWJjODUtNzY0Y2VlNTk4MzNjLyIsImlhdCI6MTQ3MTk1NDY1MywibmJmIjoxNDcxOTU0NjUzLCJleHAiOjE0NzE5NTg1NTMsImFjciI6IjEiLCJhbXIiOlsicHdkIl0sImFwcGlkIjoiZWExZTQ5MWEtYTA4OS00Njk4LTgwNTEtM2ExYjlhYWIxYThkIiwiYXBwaWRhY3IiOiIxIiwiZmFtaWx5X25hbWUiOiJNaXJvbmNodWsiLCJnaXZlbl9uYW1lIjoiU2VyZ2V5IiwiaXBhZGRyIjoiMTk1LjI0LjE0MS4xMCIsIm5hbWUiOiJzbW9yZzAxIiwib2lkIjoiZWUwOTY0NWEtZjUwNy00MTdhLThhOGEtNzA3MjlmMWYyMzhmIiwicGxhdGYiOiJXaW4iLCJwdWlkIjoiMTAwMzAwMDA5QTFGQkQyOCIsInNjcCI6IkNhbGVuZGFycy5SZWFkIENvbnRhY3RzLlJlYWQgRmlsZXMuUmVhZCBHcm91cC5SZWFkLkFsbCBNYWlsLlJlYWQgVXNlci5SZWFkIFVzZXIuUmVhZC5BbGwiLCJzdWIiOiJGenBXWHBfdDFpS08yb29XWHFFWTJIUmxqdTFiamZ6cUE5T1ZJX2VxeGRVIiwidGlkIjoiY2VmZDc4NmItNTUyMC00ZmRjLWJjODUtNzY0Y2VlNTk4MzNjIiwidW5pcXVlX25hbWUiOiJzZXJnZXlAc2VyZ2V5bS5vbm1pY3Jvc29mdC5jb20iLCJ1cG4iOiJzZXJnZXlAc2VyZ2V5bS5vbm1pY3Jvc29mdC5jb20iLCJ2ZXIiOiIxLjAifQ.JMC4PwKZgEng-0jYQO0VBmCqNlonrADJHiruo21YXwewjH3NyQOUddSRp4eFcPU0qiSbtXFcodKFCk1TUeVKm-1QT0VWSEUJ4ImIx1dPobXp486DpKAX7dXpVoijERWlL56nj3mpcpks6dk89gAizXw70aWDhEM4G5AQtxl1_a2d4VMvIWF0a0T3LSKzOyw7-YrJ-LVi32xoPC6uLmpuP9Vrm2dk6qpU2W6p7DR8FPlpZDiYmM2fuwp4RkuyYSiTv_McGEsHB9wr6pKX1_N_Icc6aknYhn2ytyI75RPeRM3xvLlmVDRXLI7tqd0jVzOCNcbkTow5dq8r_52P697a2Q";

            headers = new HeaderParameters
            {
                {"Authorization", $"Bearer {Token}"},
                {"Accept-Charset", "UTF-8" }
            };

            requestManager = new RestRequestManager(new Uri("https://graph.microsoft.com"));
            try
            {
                var drive = requestManager.Get(
                    $"v1.0/users/smuser01@sergeym.onmicrosoft.com/drive",
                   HttpRequestContentTypes.Application.Json,
                    headers);

                System.Console.WriteLine("{0}", JsonPrettify(drive));
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        public static string JsonPrettify(string json)
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

    }

    internal class Drive
    {
    }
}
