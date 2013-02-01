using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel.Composition;

namespace NuBot.Parts
{
    [Export(typeof(IPart))]
    public class TrollModule : SimplePart
    {
        private Random _rand = new Random();

        private string[] trolls = new[] {
            "http://i0.kym-cdn.com/photos/images/newsfeed/000/108/728/Oprah_umad.gif?1301082881",
            "http://t.qkme.me/3oy755.jpg",
            "http://t0.gstatic.com/images?q=tbn:ANd9GcSXI09FJxrT5lm_5o98Zu528sr6Doj0o13ChzPsB5S07aQusQmpOw"
        };

        public override string Name
        {
            get { return "Troll Module"; }
        }

        public override void Run(IRobot robo)
        {
            robo.Respond("troll me", m =>
                robo.Say(trolls[_rand.Next(0, trolls.Length - 1)], m.Room));
        }
    }
}
