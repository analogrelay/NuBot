using System.IO;

namespace NuBot.Abstractions
{
    public class PhysicalTextFile : ITextFile
    {
        private readonly string _path;
        
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
