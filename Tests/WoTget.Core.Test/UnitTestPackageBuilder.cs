using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WoTget.Core.Authoring;
using System.IO;

namespace WoTget.Core.Test
{
    [TestClass]
    [DeploymentItem("Ressources\\mods", "mods")]
    public class UnitTestPackageBuilder
    {

        public IPackage GetSAE1()
        {
            return new Package
            {
                Name = "SAE_Auto_aim_V19.5_test",
                Description = "SAE1",
                Version = "1.0"
            };
        }

        public IPackage GetSAE2()
        {
            return new Package
            {
                Name = "SAE_Auto_aim_V19.5_MaxJaar_Config",
                Description = "SAE2",
                Version = "1.0"
            };
        }

        public IPackage GetTundra()
        {
            return new Package
            {
                Name = "Tundra",
                Description = "Tundra",
                Version = "1.0"
            };
        }


        public IPackage GetShadow()
        {
            return new Package
            {
                Name = "Shadow",
                Description = "Shadow",
                Version = "1.0"
            };
        }

        private string[] GetFiles(IPackage package)
        {
            return Directory.GetFiles(Path.Combine("mods", package.Name), "*", SearchOption.AllDirectories);
        }

        private void TestPackage(IPackage package)
        {
            var destFile = package.FileName();
            using (var stream = PackageBuilder.CreatePackage(package, GetFiles(package)))
            {
                using (var fileStream = File.Create(destFile))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            Assert.IsTrue(File.Exists(destFile));

            var package2 = PackageReader.GetManifestFromPackageStream(File.OpenRead(destFile));
            Assert.IsTrue((Package)package == (Package)package2);
        }



        [TestMethod]
        public void TestMethodCreatePackageSAE1()
        {
            TestPackage(GetSAE1());
        }

        [TestMethod]
        public void TestMethodCreatePackageSAE2()
        {
            TestPackage(GetSAE2());
        }

        [TestMethod]
        public void TestMethodCreatePackageTundra()
        {
            TestPackage(GetTundra());

        }

        [TestMethod]
        public void TestMethodCreatePackageShadow()
        {
            TestPackage(GetShadow());
        }
    }
}
