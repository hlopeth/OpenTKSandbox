using System.Collections.Generic;

namespace OpenTKSandbox
{
    interface IScene
    {
        List<IModel> Models { get; }
        
        void Draw();

        void Delete();
    }
}
