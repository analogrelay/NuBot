using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Core
{
    public class Robot
    {
        public IList<IPart> Parts { get; private set; }

        public Robot()
        {
            Parts = new List<IPart>();
        }
    }
}
