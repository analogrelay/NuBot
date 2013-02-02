using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace NuBot.Infrastructure
{
    public interface ILogConfiguration
    {
        Logger CreateLogger(string name);
    }
}
