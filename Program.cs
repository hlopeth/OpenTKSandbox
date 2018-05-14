using OpenTK;

namespace OpenTKSandbox
{
    internal static class Program
    {
        private const int Width = 800;
        private const int Height = 600;
        
        public static void Main(string[] args)
        {
            var engine = new Engine(Width, Height);
            engine.Run();
        }
    }
}