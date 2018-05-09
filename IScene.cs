using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKSandbox
{
    interface IScene
    {
        List<IModel> Models { get; }
        void Draw();
    }
}
