using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuBot.Core.Messages;
using System.Text.RegularExpressions;
using NuBot.Core.Services;
using System.ComponentModel.Composition;

namespace NuBot.Core.Parts
{
    [Export(typeof(IPart))]
    public class BeesModule : SimplePart
    {
        public override string Name
        {
            get { return "Bees Module"; }
        }

        public override void Run(IRobot robo)
        {
            robo.Hear("bees", m => robo.Say("http://img37.imageshack.us/img37/7044/oprahbees.gif", m.Room));
        }
    }
}
