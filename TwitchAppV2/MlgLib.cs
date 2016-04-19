using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
//using System.IO;
using System.Text.RegularExpressions;


namespace TwitchAppV2
{
    class MlgLib
    {
        const string MLG_CONFIG_URL = "http://www.majorleaguegaming.com/player/config.json";
        const string MLG_STREAM_API_URL = "http://streamapi.majorleaguegaming.com/service/streams/playback/";
        const string STREAM_ID_PATTERN = @"<meta content='.+/([\w_-]+).+' property='og:video'>";

        HttpClient httpClient;

        public MlgLib()
        {
            httpClient = new HttpClient();
        }

        private async Task<Newtonsoft.Json.Linq.JObject> _GetStreamId(string channelId)
        {
            HttpRequestMessage reqMsg = new HttpRequestMessage();
            reqMsg.Method = HttpMethod.Get;
            reqMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            reqMsg.RequestUri = new Uri(String.Format("{0}?id={1}", MLG_CONFIG_URL, channelId));

            var respMsg = await httpClient.SendAsync(reqMsg);
            respMsg.EnsureSuccessStatusCode();

            dynamic jsObject = JsonConvert.DeserializeObject(await respMsg.Content.ReadAsStringAsync());


            return jsObject;

        }

        private async Task<Newtonsoft.Json.Linq.JObject> _GetStreamUrlJson(string streamId)
        {
            HttpRequestMessage reqMsg = new HttpRequestMessage();
            reqMsg.Method = HttpMethod.Get;
            reqMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            reqMsg.RequestUri = new Uri(String.Format("{0}/{1}?format=all", MLG_STREAM_API_URL, streamId));

            var respMsg = await httpClient.SendAsync(reqMsg);
            respMsg.EnsureSuccessStatusCode();

            dynamic jsObject = JsonConvert.DeserializeObject(await respMsg.Content.ReadAsStringAsync());


            return jsObject;
        }

        private async Task<string> _GetChannelId(string channelName)
        {
            HttpRequestMessage reqMsg = new HttpRequestMessage();
            reqMsg.Method = HttpMethod.Get;
            reqMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/html"));
            reqMsg.RequestUri = new Uri(String.Format("http://mlg.tv/{0}", channelName));
            
            var respMsg = await httpClient.SendAsync(reqMsg);
            respMsg.EnsureSuccessStatusCode();

            var webSource = await respMsg.Content.ReadAsStringAsync();
            Regex rgx = new Regex(STREAM_ID_PATTERN);

            var matches = rgx.Matches(webSource);
            string channelid = matches[0].Groups[1].Value;


            return channelid;

        }
        
        public async Task<List<string>> GetStreams(string channelName)
        {
            var channelId = await _GetChannelId(channelName);
            var jsResponse = await _GetStreamId(channelId);
            string streamId = null;
            string hlsM3uUrl = null;
            List<string> m3u = null; 
            if (jsResponse["media"].HasValues)
            {
                streamId = (string)jsResponse["media"][0]["channel"];
            }

            if (streamId != null)
            {
                var streamJson = await _GetStreamUrlJson(streamId);

                if (streamJson["data"].HasValues)
                {
                    foreach (var item in streamJson["data"]["items"])
                    {
                        if ((string)item["format"] == "hls")
                        {
                            hlsM3uUrl = (string)item["url"];

                            HttpRequestMessage reqMsg = new HttpRequestMessage();
                            reqMsg.Method = HttpMethod.Get;
                            reqMsg.RequestUri = new Uri(hlsM3uUrl);
                            var respMsg = await httpClient.SendAsync(reqMsg);
                            respMsg.EnsureSuccessStatusCode();

                            var channelAvailableStreams = await respMsg.Content.ReadAsStringAsync();

                            m3u = new List<string>(Regex.Split(channelAvailableStreams, "\n"));
                        }
                    }
                }
            }
            return m3u;
        }
    }
}
