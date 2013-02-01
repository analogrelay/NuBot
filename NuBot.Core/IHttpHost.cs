using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace NuBot
{
    public interface IHttpHost
    {
        string Name { get; }
        void StartServer(IRobot robo, Action<IAppBuilder> app);
    }
}
