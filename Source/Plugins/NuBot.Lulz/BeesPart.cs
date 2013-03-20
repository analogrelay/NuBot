using System.Threading;

namespace NuBot.Lulz
{
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
