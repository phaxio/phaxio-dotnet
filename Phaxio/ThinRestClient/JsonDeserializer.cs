using Newtonsoft.Json;

namespace Phaxio.ThinRestClient
{
    public class JsonDeserializer
    {
        public T Deserialize<T>(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}
