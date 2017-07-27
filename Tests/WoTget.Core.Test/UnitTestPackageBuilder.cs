using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WoTget.Core.Authoring;
using System.IO;
using System.Collections.Generic;

namespace WoTget.Core.Test
{
    [TestClass]
    [DeploymentItem("Ressources\\mods", "mods")]
    public class UnitTestPackageBuilder
    {

        public TestContext TestContext { get; set; }

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

        public IPackage GetLasers()
        {
            return new Package
            {
                Name = "9.19 Lasers By PolarFox",
                Description = "Lasers by PolarFox",
                Version="1.0"
            };
        }

        private string[] GetFiles(IPackage package)
        {
            return Directory.GetFiles(Path.Combine("mods", package.Name), "*", SearchOption.AllDirectories);
        }

        private void TestPackage(IPackage package)
        {
            string destFile;
            using (var stream = PackageBuilder.CreatePackage(package, GetFiles(package), Path.Combine("mods", package.Name)))
            {
                destFile = package.FileName();
                using (var fileStream = File.Create(destFile))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            Assert.IsTrue(File.Exists(destFile));
            TestContext.AddResultFile(destFile);

            var package2 = PackageReader.GetMetaFromPackageStream(File.OpenRead(destFile));
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

        [TestMethod]
        public void TestMethodCreatePackageLasers()
        {
            TestPackage(GetLasers());
        }

        [TestMethod]
        public void TestMethodCreatePackageShadowZip()
        {

            string destFile;
            var package = GetShadow();
            using (var stream = PackageBuilder.CreatePackage(package, new List<string>() { Path.Combine("mods", "Shadow.zip") }, "mods"))
            {
                destFile = "Zip_Shadow.wotget";
                using (var fileStream = File.Create(destFile))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            Assert.IsTrue(File.Exists(destFile));
            TestContext.AddResultFile(destFile);

            var package2 = PackageReader.GetMetaFromPackageStream(File.OpenRead(destFile));
            Assert.IsTrue((Package)package == (Package)package2);
        }

        [TestMethod]
        public void TestMethodCreatePackageTundraZip()
        {
            string destFile;
            var package = GetTundra();
            using (var stream = PackageBuilder.CreatePackage(package, new List<string>() { Path.Combine("mods", "Tundra.zip") }, "mods"))
            {
                destFile = "Zip_Tundra.wotget";
                using (var fileStream = File.Create(destFile))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            Assert.IsTrue(File.Exists(destFile));
            TestContext.AddResultFile(destFile);

            var package2 = PackageReader.GetMetaFromPackageStream(File.OpenRead(destFile));
            Assert.IsTrue((Package)package == (Package)package2);
        }

        [TestMethod]
        public void TestMethodCreatePackageSAE2Zip()
        {
            string destFile;
            var package = GetSAE2();
            using (var stream = PackageBuilder.CreatePackage(package, new List<string>() { Path.Combine("mods", "SAE_Auto_aim_V19.5_MaxJaar_Config.zip") }, "mods"))
            {
                destFile = "Zip_SAE2.wotget";
                using (var fileStream = File.Create(destFile))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            Assert.IsTrue(File.Exists(destFile));
            TestContext.AddResultFile(destFile);

            var package2 = PackageReader.GetMetaFromPackageStream(File.OpenRead(destFile));
            Assert.IsTrue((Package)package == (Package)package2);
        }

        [TestMethod]
        public void TestMethodCreatePackageDestroyedObjectsZip()
        {
            string destFile;
            var package = GetSAE2();
            using (var stream = PackageBuilder.CreatePackage(package, new List<string>() { Path.Combine("mods", "9.19.1 Destroyed Objects On The Minimap And InGame By PolarFox.zip") }, "mods"))
            {
                destFile = "Zip_DO.wotget";
                using (var fileStream = File.Create(destFile))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            Assert.IsTrue(File.Exists(destFile));
            TestContext.AddResultFile(destFile);

            var package2 = PackageReader.GetMetaFromPackageStream(File.OpenRead(destFile));
            Assert.IsTrue((Package)package == (Package)package2);
        }



    }
}
