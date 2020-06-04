using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SellGirl.Utils.Ionic.Zip;
//using SellGirl.Utils.ZipLib.Zip;

namespace SellGirl.Core
{
    public class ZipCompress : ICompress
    {
        public string Suffix(string orgSuffix)
        {
            return "zip";
        }

        public Stream Compress(Stream fileStream, string fullName)
        {
            using (var zip = new ZipFile())
            {
                zip.AddEntry(fullName, fileStream);
                Stream zipStream = new MemoryStream();
                zip.Save(zipStream);
                return zipStream;
            }
        }
    }
}