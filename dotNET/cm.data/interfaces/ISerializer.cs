
namespace cm.data.interfaces
{
    public interface ISerializer
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string data);
    }
}
