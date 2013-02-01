using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuBot.Abstractions;

namespace NuBot.Test.Abstractions
{
    public class TestTextFile : ITextFile
    {
        private string _content;

        public TestTextFile(string content)
        {
            _content = content;
        }

        public string ReadAllText()
        {
            return _content;
        }
    }
}
