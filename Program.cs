using OpenTK;
using OpenTK.Graphics;

namespace OpenTKSandbox
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var window = new GameWindow(800, 600, GraphicsMode.Default, "Sandbox");
            var engine = new Engine(window);
            window.Run();
        }
    }
}