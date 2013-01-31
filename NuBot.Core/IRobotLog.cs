using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuBot.Core
{
    public interface IRobotLog
    {
        void Info(string message, params object[] args);
        void Error(string message, params object[] args);
        void Trace(string message, params object[] args);
    }
}
