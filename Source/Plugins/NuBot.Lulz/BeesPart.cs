using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel.Composition;

namespace NuBot.Lulz
{
    [Export(typeof(IPart))]
    public class BeesPart : Part
    {
        public override string Name
        {
            get { return "lulz.bees"; }
        }

        public override string Title
        {
            get { return "Bees! Part"; }
        }

        public override void Attach(IRobot robo, CancellationToken token)
        {
            robo.Hear("bees", m => robo.Say("http://img37.imageshack.us/img37/7044/oprahbees.gif", m.Room));
        }
    }
}
