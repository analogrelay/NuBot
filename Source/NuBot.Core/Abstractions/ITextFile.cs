using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Abstractions
{
    public interface ITextFile
    {
        bool Exists { get; }
        string ReadAllText();
    }
}
