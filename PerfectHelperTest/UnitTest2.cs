using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using Perfect;
using System.IO;

namespace PerfectHelperTest
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            var aa = PFTaskHelper.CheckMessageInterval;
            return;
            string checkCode = PFValidateCode.GenerateCheckCode();

            byte[] bytes = PFValidateCode.CreateCheckCodeImage(checkCode);
            System.IO.File.WriteAllBytes(string.Format("{0}/{1}",PFDataHelper.BaseDirectory,checkCode+".jpg"), bytes);
        }
    }
}
