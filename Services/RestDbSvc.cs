using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.IO;
using System.Text;

namespace Facilicase2.Services
{

    // this service is called by any and all methods and classes that need to make a request to the rest database

    public class RestDbSvc
    {
        // create http client initialized null
        private static HttpClient http = null;


        // this method submits a put request taking in an object of type T and returning an http response containing
        // the associated information
        public async Task<HttpResponseMessage> Put<T>(string url, T item)
        {
            if (http == null) http = new HttpClient();

            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes("username:password")));


            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var r = await http.PutAsJsonAsync(url, item);
            string str = await r.Content.ReadAsStringAsync();

            return r;
        }

        // this method submits a post request taking in an object of type T and returning an http response containing
        // the associated information
        public async Task<HttpResponseMessage> Post<T>(string url, T item)
        {
            if (http == null) http = new HttpClient();

            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes("username:password")));


            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var r = await http.PostAsJsonAsync(url, item);
            string str = await r.Content.ReadAsStringAsync();

            return r;
        }

        public async Task<HttpResponseMessage> Patch<T>(string url, T item)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes("username:password")));


                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //var writer = new StringWriter();
                //Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                //serializer.Serialize(writer, item);
                //// var json = writer.ToString();

                //string json = "{\"test\":\"test\"}";

                //StringContent s = new StringContent(json, Encoding.UTF8, "application/json");

                //request.Content = s;



                // deserialize item the json

                string json = JsonSerializer.Serialize<T>(item);

                StringContent s = new StringContent(json, Encoding.UTF8, "application/json");

                request.Content = s;

                var response = await client.SendAsync(request);

                string str = await response.Content.ReadAsStringAsync();

                return response;
            }
        }


        // this method submits a get request taking in an object of type T and returning an http response containing
        // the associated information
        public async Task<T> Get<T>(string url, bool no_cache = false)
        {
            var r = await MakeRequest(HttpMethod.Get, url, no_cache);

            // auto logout on 401 response
            if (r.StatusCode == System.Net.HttpStatusCode.Unauthorized) return default(T);

            // throw exception on error response
            if (!r.IsSuccessStatusCode)
            {
                var error = await r.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return default(T);
            }

            return await r.Content.ReadFromJsonAsync<T>();
        }

        // this method is called by get to actually get the information by making a request
        public async Task<HttpResponseMessage> MakeRequest(HttpMethod method, string url, bool no_cache)
        {
            if (http == null) http = new HttpClient();

            var request = new HttpRequestMessage(method, url);
            if (no_cache)
            {
                request.Headers.CacheControl.NoCache = true;
            }
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes("username:password")));

            return await http.SendAsync(request);
        }
    }
}
