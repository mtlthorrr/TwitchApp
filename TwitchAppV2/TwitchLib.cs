using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;

namespace TwitchAppV2
{
    class TwitchLib   
    {
        const string TWITCH_BASE_URL = "https://api.twitch.tv/kraken/";
        const string USHER_BASE_URL = "http://usher.twitch.tv/";
        const string TWITCH_API_BASE_URL = "https://api.twitch.tv/api/";

        public string AppKey { get; set; }



        HttpClient httpClient;
        
        public TwitchLib(string appKey)
        {
            AppKey = String.Format("clientid={0}", appKey);

            httpClient = new HttpClient();


        }

        // Consider building the query string, given an dictionary of arguments...
        private async Task<HttpResponseMessage> _KrakenRequest(string resource)
        {
            
                HttpRequestMessage reqMsg = new HttpRequestMessage();
                //reqMsg.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v3+json");
                reqMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v3+json"));
                reqMsg.Method = HttpMethod.Get;
                reqMsg.RequestUri = new Uri(String.Format("{0}{1}?{2}", TWITCH_BASE_URL, resource, AppKey));

                var respMsg = await httpClient.SendAsync(reqMsg);
                respMsg.EnsureSuccessStatusCode();

                return respMsg;
        }

        public async Task<Newtonsoft.Json.Linq.JObject> KrakenRequest(string resouce)
        {
            var respMsg = await _KrakenRequest(resouce);

            dynamic jsObject = JsonConvert.DeserializeObject(await respMsg.Content.ReadAsStringAsync());

            return jsObject;

        }

        private async Task<HttpResponseMessage> _UsherRequest(string resource)
        {
            HttpRequestMessage reqMsg = new HttpRequestMessage();
            reqMsg.Method = HttpMethod.Get;
            reqMsg.RequestUri = new Uri(String.Format("{0}{1}", USHER_BASE_URL, resource));

            var respMsg = await httpClient.SendAsync(reqMsg);
            respMsg.EnsureSuccessStatusCode();

            return respMsg;
        }

        public async Task<dynamic> UsherRequest(string resource, string type = null)
        {
            var respMsg = await _UsherRequest(resource);

            if (type == "select")
            {
                return await respMsg.Content.ReadAsStringAsync();
            }

            // This may be json who knows with the 40billion different api urls
            else 
            {
                dynamic jsObject = JsonConvert.DeserializeObject(await respMsg.Content.ReadAsStringAsync());
                return jsObject;
            }
        }

        //public async Task<HttpResponseMessage> ApiRequest(string resource)
        //{
        //    HttpRequestMessage reqMsg = new HttpRequestMessage();
        //    reqMsg.Method = HttpMethod.Get;
        //    reqMsg.RequestUri = new Uri(String.Format("{0}{1}", TWITCH_API_BASE_URL, resource));

        //    var respMsg = await httpClient.SendAsync(reqMsg);
        //    respMsg.EnsureSuccessStatusCode();

        //    return respMsg;
        //}

        private async Task<HttpResponseMessage> ChannelToken(string channelName)
        {
            HttpRequestMessage reqMsg = new HttpRequestMessage();
            reqMsg.Method = HttpMethod.Get;
            reqMsg.RequestUri = new Uri(String.Format("{0}channels/{1}/access_token", TWITCH_API_BASE_URL, channelName));

            var respMsg = await httpClient.SendAsync(reqMsg);
            respMsg.EnsureSuccessStatusCode();

            return respMsg;
        }

        public async Task<TwitchChannelToken> GetChannelToken(string channelName)
        {
            var respMsg = await ChannelToken(channelName);
            var jsonResp = await respMsg.Content.ReadAsStringAsync();

            var chanToken = JsonConvert.DeserializeObject<TwitchChannelToken>(jsonResp);

            return chanToken;
        }

    }

    public class TwitchChannelToken
    {
        [JsonProperty("token")]
        public string _Token { get; set; }

        [JsonProperty("sig")]
        public string _Sig { get; set; }
    }
}
