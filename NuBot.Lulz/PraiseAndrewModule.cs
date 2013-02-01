using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Lulz
{
    [Export(typeof(IPart))]
    public class PraiseAndrewModule : Part
    {
        public override string Name
        {
            get { return "Praise Andrew Module"; }
        }

        public override void Attach(IRobot robo, System.Threading.CancellationToken token)
        {
            robo.Hear("nurse", m => Praise(robo, m));
            robo.Hear("AndrewNurse", m => Praise(robo, m));
            robo.Hear("Andrew", m => Praise(robo, m));
        }

        private static void Praise(IRobot robo, Messages.ChatMessage m)
        {
            robo.Say("Praise the almighty Creator!", m.Room);
            robo.SendSlashMe("Bows", m.Room);
        }
    }
}
