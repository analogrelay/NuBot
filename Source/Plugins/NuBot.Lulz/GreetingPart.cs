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
    public class GreetingPart : Part
    {
        private readonly string[] _his = new[] {
            "Hello {0}!",
            "Bonjour {0}",
            "Howdy {0}",
            "How's it going {0}",
            "Whazzup {0}",
            "Yo {0}"
        };

        private readonly Random _rand = new Random();

        public override string Name
        {
            get { return "lulz.hi"; }
        }

        public override string Title
        {
            get { return "Greeting Module"; }
        }

        public override void Attach(IRobot robo, CancellationToken token)
        {
            robo.Respond(new[] { "Hi", "Hello", "you there" }, m => robo.Say(String.Format(_his[_rand.Next(0, _his.Length - 1)], m.From), m.Room));
        }
    }
}
