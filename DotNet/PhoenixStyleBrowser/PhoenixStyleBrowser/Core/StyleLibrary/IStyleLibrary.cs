using System.Threading.Tasks;

namespace PhoenixStyleBrowser
{
    public interface IStyleLibrary
    {
        string Name { get; }
        string Icon { get; }
        string Directory { get; }
        string Paths { get; }
        string ErrorMessage { get; }
        bool IsValid { get; }
        bool IsSelected { get; }
        AsyncCommand LoadLibrary { get; }
        Task Initialize();
    }
}
