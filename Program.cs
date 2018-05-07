using OpenTK;
using OpenTK.Graphics;

namespace OpenTKSandbox
{
    internal static class Program
    {
        private const int Width = 800;
        private const int Height = 600;
        
        public static void Main(string[] args)
        {
            var window = new GameWindow(Width, Height, GraphicsMode.Default, "Sandbox");
            var engine = new Engine(window);
            window.Run();
        }
    }
}