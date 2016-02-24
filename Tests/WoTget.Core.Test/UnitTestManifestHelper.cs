using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WoTget.Core.Authoring;
using System.IO;

namespace WoTget.Core.Test
{
    [TestClass]
    public class UnitTestManifestHelper
    {
        [TestMethod]
        [DeploymentItem("Ressources\\Manifest.xml", "Ressources")]
        public void TestMethodLoadFromPath()
        {
            var manifest = ManifestHelper.Load("Ressources\\Manifest.xml");
            Assert.IsNotNull(manifest);
        }

        [TestMethod]
        public void TestMethodLoadFromString()
        {
            var manifest = ManifestHelper.LoadFromString("<?xml version=\"1.0\" encoding=\"utf-8\" ?><package>  <name>PackageName</name>  <description>Das ist eine Beschreigung des Packages</description>  <version>1.0</version>  <authors></authors>  <owners></owners>  <projectUrl></projectUrl>  <tags>    <tag>tag</tag>    <tag>tag2</tag>  </tags></package>");
            Assert.IsNotNull(manifest);
        }

        [TestMethod]
        [DeploymentItem("Ressources\\Manifest.xml", "Ressources")]
        public void TestMethodSaveFile()
        {
            var manifest = ManifestHelper.Load("Ressources\\Manifest.xml");
            ManifestHelper.Save(manifest, "Manifest.xml");
            Assert.IsTrue(File.Exists("Manifest.xml"));
        }

        [TestMethod]
        [DeploymentItem("Ressources\\Manifest.xml", "Ressources")]
        public void TestMethodSaveStream()
        {
            var manifest = ManifestHelper.Load("Ressources\\Manifest.xml");
            using (var stream = new FileStream("Manifest.xml", FileMode.Create, FileAccess.Write))
            {
                ManifestHelper.Save(manifest,stream);
            }
            Assert.IsTrue(File.Exists("Manifest.xml"));
        }
    }
}
