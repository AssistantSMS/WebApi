using ScrapMechanic.Domain.Result;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScrapMechanic.Integration.Repository
{
    public class BaseExternalApiRepository
    {
        protected async Task<ResultWithValue<T>> Get<T>(string url, Action<HttpRequestHeaders> manipulateHeaders = null)
        {
            ResultWithValue<string> webGetResult = await Get(url, manipulateHeaders);
            if (webGetResult.HasFailed) return new ResultWithValue<T>(false, default, webGetResult.ExceptionMessage);

            try
            {
                T result = JsonConvert.DeserializeObject<T>(webGetResult.Value);
                return new ResultWithValue<T>(true, result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResultWithValue<T>(false, default, ex.Message);
            }
        }

        protected async Task<ResultWithValue<string>> Get(string url, Action<HttpRequestHeaders> manipulateHeaders = null)
        {
            HttpClient client = new HttpClient();
            try
            {
                manipulateHeaders?.Invoke(client.DefaultRequestHeaders);
                HttpResponseMessage httpResponse = await client.GetAsync(url);
                string content = await httpResponse.Content.ReadAsStringAsync();
                return new ResultWithValue<string>(true, content, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResultWithValue<string>(false, default, ex.Message);
            }
            finally
            {
                client.Dispose();
            }
        }

        //protected async Task<ResultWithValue<string>> Post(string url, string postContent)
        //{
        //    HttpClient client = new HttpClient();
        //    try
        //    {
        //        HttpResponseMessage httpResponse = await client.PostAsync(url, postContent);
        //        string content = await httpResponse.Content.ReadAsStringAsync();
        //        return new ResultWithValue<string>(true, content, string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultWithValue<string>(false, default, ex.Message);
        //    }
        //    finally
        //    {
        //        client.Dispose();
        //    }
        //}
    }
}
