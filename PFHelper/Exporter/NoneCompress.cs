using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Perfect
{
    public class NoneCompress : ICompress
    {
        public string Suffix(string orgSuffix)
        {
            return orgSuffix;
        }

        public Stream Compress(Stream fileStream, string fullName)
        {
            return fileStream;
        }
    }
}
