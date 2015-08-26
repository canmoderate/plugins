using System.Web.Script.Serialization;
using cm.data.interfaces;

namespace cm.net4 
{
    public class Serializer : ISerializer
    {
        private readonly JavaScriptSerializer _jsonSerializer;

        public Serializer()
        {
            _jsonSerializer = new JavaScriptSerializer();
        }

        public string Serialize<T>(T value)
        {
            return _jsonSerializer.Serialize(value);
        }

        public T Deserialize<T>(string json)
        {
            return _jsonSerializer.Deserialize<T>(json);
        }
    }
}
