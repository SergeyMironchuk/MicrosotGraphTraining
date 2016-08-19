// Copyright (c) 2016, Virtuworks.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must  retain  the  above copyright notice, this
//   list of conditions and the following disclaimer.
//
// - Redistributions in binary form  must  reproduce the  above  copyright  notice,
//   this list of conditions  and  the  following  disclaimer in  the documentation
//   and/or other materials provided with the distribution.
//
// - Neither  the  name  of  Virtuworks   nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft_Graph_SDK_ASPNET_Connect.Controllers.Extensions;

namespace Microsoft_Graph_SDK_ASPNET_Connect.Controllers.Http
{
    public class RestRequestManager
    {
        private readonly Uri _baseUrl;

        public RestRequestManager(Uri baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<T> Put<T>(string uriRequest, string contentType, HeaderParameters headerParameters, T bodyObject)
        {
            using (var client = new HttpClient())
            {
                InitRestClient(contentType, headerParameters, client);

                HttpResponseMessage response = await client.PutAsJsonAsync(uriRequest, bodyObject);

                if (!response.IsSuccessStatusCode)
                {
                    HandleErrorResponse(response);
                }

                return await response.Content.ReadAsAsync<T>();
            }
        }

        public async Task<T> Patch<T>(string uriRequest, string contentType, HeaderParameters headerParameters, T bodyObject)
        {
            using (var client = new HttpClient())
            {
                InitRestClient(contentType, headerParameters, client);

                HttpResponseMessage response = await client.PatchAsJsonAsync(uriRequest, bodyObject);

                if (!response.IsSuccessStatusCode)
                {
                    HandleErrorResponse(response);
                }

                return await response.Content.ReadAsAsync<T>();
            }
        }

        public async Task<T> Post<T>(string uriRequest, string contentType, HeaderParameters headerParameters, T bodyObject)
        {
            using (var client = new HttpClient())
            {
                InitRestClient(contentType, headerParameters, client);

                HttpResponseMessage response = await client.PostAsJsonAsync(uriRequest, bodyObject);

                if (!response.IsSuccessStatusCode)
                {
                    HandleErrorResponse(response);
                }

                return await response.Content.ReadAsAsync<T>();
            }
        }


        public T Post<T>(string uriRequest, string contentType, HeaderParameters headerParameters, FormUrlEncodedContent encodedContent)
        {
            using (var client = new HttpClient())
            {
                InitRestClient(contentType, headerParameters, client);

                HttpResponseMessage response = client.PostAsync(uriRequest, encodedContent).Result;

                //if (!response.IsSuccessStatusCode)
                //{
                //    HandleErrorResponse(response);
                //}

                return response.Content.ReadAsAsync<T>().Result;
            }
        }

        public async Task<T> Get<T>(string uriRequest, string contentType, HeaderParameters headerParameters)
        {
            using (var client = new HttpClient())
            {
                InitRestClient(contentType, headerParameters, client);

                HttpResponseMessage response = await client.GetAsync(uriRequest);

                if (!response.IsSuccessStatusCode)
                {
                    HandleErrorResponse(response);
                }
                
                var textResponse = response.Content.ReadAsStringAsync().Result;

                return await response.Content.ReadAsAsync<T>();
            }
        }

        public string Get(string uriRequest, string contentType, HeaderParameters headerParameters)
        {
            using (var client = new HttpClient())
            {
                InitRestClient(contentType, headerParameters, client);

                HttpResponseMessage response = client.GetAsync(uriRequest).Result;

                //if (!response.IsSuccessStatusCode)
                //{
                //    HandleErrorResponse(response);
                //}

                var textResponse = response.Content.ReadAsStringAsync().Result;

                return textResponse;
            }
        }

        protected void HandleErrorResponse(HttpResponseMessage response)
        {
        }

        private void InitRestClient(string contentType, HeaderParameters headerParameters, HttpClient client)
        {
            client.BaseAddress = _baseUrl;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            if (headerParameters != null)
            {
                foreach (var headerParameter in headerParameters)
                {
                    client.DefaultRequestHeaders.Add(headerParameter.Key, headerParameter.Value);
                }
            }
        }
    }
}
