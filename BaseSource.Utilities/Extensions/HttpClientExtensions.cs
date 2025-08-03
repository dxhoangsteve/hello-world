using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Utilities.Helper
{
    public static class HttpClientExtensions
    {
        public static async Task<T> PostAsync<T>(this HttpClient client, string url, object data = null, string bearerToken = null)
        {
            if (!string.IsNullOrEmpty(bearerToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            var json = JsonConvert.SerializeObject(data);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                using (var response = await client.PostAsync(url, content))
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<T>(responseString);
                    return result;
                }
            }
        }

        public static async Task<T> PostFormAsync<T>(this HttpClient client, string url, HttpContent content = null)
        {
            using (var response = await client.PostAsync(url, content))
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(responseString);
                return result;
            }
        }

        public static async Task<T> PostAsyncFormUrl<T>(this HttpClient client, string url, Dictionary<string, string> data)
        {
            using (var content = new FormUrlEncodedContent(data))
            {
                using (var response = await client.PostAsync(url, content))
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<T>(responseString);
                    return result;
                }
            }
        }

        public static async Task<T> GetAsync<T>(this HttpClient client, string url, object data = null)
        {
            if (data != null)
            {
                string queryString = UrlHelper.GetQueryString(data);
                url += "?" + queryString;
            }

            using (var response = await client.GetAsync(url))
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(responseString);
                return result;
            }
        }

        public static async Task<byte[]> GetFileAsync(this HttpClient client, string url, object data = null)
        {
            if (data != null)
            {
                string queryString = UrlHelper.GetQueryString(data);
                url += "?" + queryString;
            }

            using (var response = await client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                    return null;

                // Đọc dữ liệu byte[] từ API
                var fileBytes = await response.Content.ReadAsByteArrayAsync();

                // Trả file về cho user
                return fileBytes;
            }
        }
    }
}
