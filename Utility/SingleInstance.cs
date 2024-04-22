#pragma warning disable 8618

namespace Utility
{
    public interface IClose
    {
        void Close();
    }

    public class SingleInstance<T>
        where T : class, IClose, new()
    {
        static protected T s_Instance;
        static public T Instance { get => s_Instance; }
        static public void Initialize() { if (s_Instance == null) s_Instance = new T(); }
        static public void Close() { if (s_Instance != null) s_Instance.Close(); }
    }
}
