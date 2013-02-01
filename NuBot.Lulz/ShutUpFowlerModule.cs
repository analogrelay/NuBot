using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Lulz
{
    // Disable this... for now ;)
    [Export(typeof(IPart))]
    public class ShutUpFowlerModule : Part
    {
        public override string Name
        {
            get { return "Shut Up Fowler Module"; }
        }

        public override void Attach(IRobot robo, System.Threading.CancellationToken token)
        {
            robo.Respond(m =>
            {
                if (String.Equals(m.From, "dfowler"))
                {
                    robo.Say("Shut up Fowler!", m.Room);
                }
            });
            robo.Hear("fowler", m => {
                robo.Say("*sigh* Fowler...", m.Room);
                robo.SendSlashMe("Rolls Eyes", m.Room);
            });
        }
    }
}
