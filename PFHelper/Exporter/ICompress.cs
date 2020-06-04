using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Perfect
{
    public interface ICompress
    {
        string Suffix(string orgSuffix);
        Stream Compress(Stream fileStream, string fullName);
    }
}
