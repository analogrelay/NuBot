using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Abstractions
{
    public class PhysicalTextFile : ITextFile
    {
        private string _path;
        
        public bool Exists
        {
            get { return File.Exists(_path); }
        }

        public PhysicalTextFile(string path)
        {
            _path = path;
        }

        public string ReadAllText()
        {
            return File.ReadAllText(_path);
        }
    }
}
