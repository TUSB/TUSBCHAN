using Newtonsoft.Json;

namespace TUSBちゃん.API.Docomo
{
    class ChatResponse
    {
        [JsonProperty("utt")]
        public string utt { get; set; }

        [JsonProperty("yomi")]
        public string yomi { get; set; }

        [JsonProperty("mode")]
        public string mode { get; set; }

        [JsonProperty("da")]
        public string da { get; set; }

        [JsonProperty("context")]
        public string context { get; set; }
    }
}
