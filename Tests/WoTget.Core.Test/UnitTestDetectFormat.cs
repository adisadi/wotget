using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using WoTget.Core.Format;
using static WoTget.Core.Format.FormatDetect;

namespace WoTget.Core.Test
{
    [TestClass]
    [DeploymentItem("Ressources\\mods", "mods")]
    public class UnitTestDetectFormat
    {
        //old res_mods unzipped
        [TestMethod]
        public void TestSAE1()
        {
            var result = FormatDetect.Analize(Directory.GetFiles(Path.Combine("mods", "SAE_Auto_aim_V19.5_MaxJaar_Config"), "*", SearchOption.AllDirectories));
            Assert.AreEqual(result, PackageFormat.WotHomeRoot);
        }

        [TestMethod]
        public void TestShadow()
        {
            var result = FormatDetect.Analize(Directory.GetFiles(Path.Combine("mods", "Shadow"), "*", SearchOption.AllDirectories));
            Assert.AreEqual(result, PackageFormat.VersionRoot);
        }

        [TestMethod]
        public void TestTundra()
        {
            var result = FormatDetect.Analize(Directory.GetFiles(Path.Combine("mods", "Tundra"), "*", SearchOption.AllDirectories));
            Assert.AreEqual(result, PackageFormat.ResModRoot);
        }

        //old res_mods zipped
        [TestMethod]
        public void TestSAE1Zip()
        {
            var result = FormatDetect.Analize(new List<string>() { Path.Combine("mods", "SAE_Auto_aim_V19.5_MaxJaar_Config.zip") });
            Assert.AreEqual(result, PackageFormat.ZipFileWotHomeRoot);
        }


        [TestMethod]
        public void TestShadowZip()
        {
            var result = FormatDetect.Analize(new List<string>() { Path.Combine("mods", "Shadow.zip") });
            Assert.AreEqual(result, PackageFormat.ZipFileVersionRoot);
        }

        [TestMethod]
        public void TestTundraZip()
        {
            var result = FormatDetect.Analize(new List<string>() { Path.Combine("mods", "Tundra.zip") });
            Assert.AreEqual(result, PackageFormat.ZipFileResModRoot);
        }

        // new mods
        [TestMethod]
        public void TestDestroyedObjects()
        {
            var result = FormatDetect.Analize(Directory.GetFiles(Path.Combine("mods", "9.19.1 Destroyed Objects On The Minimap And InGame By PolarFox"), "*", SearchOption.AllDirectories));
            Assert.AreEqual(result, PackageFormat.WotModHomeRoot);
        }

        [TestMethod]
        public void TestDestroyedObjectsZip()
        {
            var result = FormatDetect.Analize(new List<string>() { Path.Combine("mods", "9.19.1 Destroyed Objects On The Minimap And InGame By PolarFox.zip") });
            Assert.AreEqual(result, PackageFormat.WotModZipHomeRoot);
        }
    }
}
