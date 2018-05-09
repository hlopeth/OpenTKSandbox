using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKSandbox
{
    class Scene : IScene
    {
        private List<IModel> models;
        public Scene()
        {
            models = new List<IModel>();
        }

        public void AddModel(IModel model) => models.Add(model);

        public void RemoveModel(IModel model) => models.Remove(model);

        public void Draw()
        {
            foreach (var model in models)
                model.Draw();
        }

        public List<IModel> Models
        {
            get { return models; }
        }
    }
}
