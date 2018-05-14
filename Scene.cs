using System.Collections.Generic;

namespace OpenTKSandbox
{
    class Scene : IScene
    {
        public List<IModel> Models { get; }
        
        public Scene()
        {
            Models = new List<IModel>();
        }

        public void Draw()
        {
            foreach (var model in Models)
                model.Draw();
        }

        public void Delete()
        {
            foreach (var model in Models)
            {
                model.Dispose();
            }
        }
    }
}
