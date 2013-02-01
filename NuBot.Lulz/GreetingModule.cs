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
    public class GreetingModule : Part
    {
        private string[] _his = new[] {
            "Hello {0}!",
            "Bonjour {0}",
            "Howdy {0}",
            "How's it going {0}",
            "Whazzup {0}",
            "Yo {0}"
        };

        private Random _rand = new Random();

        public override string Name
        {
            get { return "Greeting Module"; }
        }

        public override void Attach(IRobot robo, CancellationToken token)
        {
            robo.Respond("Hi", m => {
                robo.Say(String.Format(_his[_rand.Next(0, _his.Length - 1)], m.From), m.Room);
            });
            robo.Respond("Hello", m =>
            {
                robo.Say(String.Format(_his[_rand.Next(0, _his.Length - 1)], m.From), m.Room);
            });
        }
    }
}
