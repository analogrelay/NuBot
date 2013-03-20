using System;
using System.Threading;
using System.ComponentModel.Composition;

namespace NuBot.Parts
{
    [Export(typeof(IPart))]
    public class TrollPart : Part
    {
        private readonly Random _rand = new Random();

        private readonly string[] _trolls = new[] {
            "http://i0.kym-cdn.com/photos/images/newsfeed/000/108/728/Oprah_umad.gif?1301082881",
            "http://t.qkme.me/3oy755.jpg",
            "http://t0.gstatic.com/images?q=tbn:ANd9GcSXI09FJxrT5lm_5o98Zu528sr6Doj0o13ChzPsB5S07aQusQmpOw"
        };

        public override string Name
        {
            get { return "lulz.trollme"; }
        }

        public override string Title
        {
            get { return "Troll Module"; }
        }

        public override void Attach(IRobot robo, CancellationToken token)
        {
            robo.Respond("troll me", m =>
                robo.Say(_trolls[_rand.Next(0, _trolls.Length - 1)], m.Room));
        }
    }
}
