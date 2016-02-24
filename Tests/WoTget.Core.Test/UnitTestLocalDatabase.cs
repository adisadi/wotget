using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using WoTget.Core.Authoring;
using WoTget.Core.Database;
using WoTget.Core.Installer;

namespace WoTget.Core.Test
{
    [TestClass]
    [DeploymentItem("Ressources\\mods", "mods")]
    public class UnitTestLocalDatabase
    {

        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            if (!Directory.Exists("wot_home\\res_mods")) Directory.CreateDirectory("wot_home\\res_mods");

            var database = new LocalDatabase("database", new PackageInstaller());
            database.Init("wot_home", "0.9.13", false);

        }

        [TestMethod]
        public void TestMethodInit()
        {
            var database = new LocalDatabase("database", new PackageInstaller());
            Assert.IsTrue(database.Exists && database.WoTHome== "wot_home" && database.WoTVersion== "0.9.13");
        }

        [TestMethod]
        public void TestMethodInstallPackages()
        {
            EnsureInstalledTestPackages();

            var testPackages = new UnitTestPackageBuilder();
            CheckInstallationFiles(testPackages.GetSAE2());
            CheckInstallationFiles(testPackages.GetTundra());
        }

        [TestMethod]
        public void TestMethodRemovePackageSAE1()
        {
            var database = new LocalDatabase("database", new PackageInstaller());


            EnsureInstalledTestPackages();

            var testPackages = new UnitTestPackageBuilder();
            var testp = testPackages.GetSAE1();
            database.UninstallPackage(testp);

            CheckInstallationFiles(testPackages.GetSAE2());
            CheckInstallationFiles(testPackages.GetTundra());

            Assert.IsTrue(true);

        }

        [TestMethod]
        public void TestMethodRemovePackageSAE2()
        {
            var database = new LocalDatabase("database", new PackageInstaller());


            EnsureInstalledTestPackages();

            var testPackages = new UnitTestPackageBuilder();
            var testp = testPackages.GetSAE2();
            database.UninstallPackage(testp);

            CheckInstallationFiles(testPackages.GetSAE1());
            CheckInstallationFiles(testPackages.GetTundra());

            Assert.IsTrue(true);

        }


        [TestMethod]
        public void TestMethodRemovePackageShadow()
        {
            var database = new LocalDatabase("database", new PackageInstaller());


            EnsureInstalledTestPackages();

            var testPackages = new UnitTestPackageBuilder();
            var testp = testPackages.GetShadow();
            database.UninstallPackage(testp);

            CheckInstallationFiles(testPackages.GetSAE2());
            CheckInstallationFiles(testPackages.GetTundra());

            Assert.IsTrue(true);

        }

        [TestMethod]
        public void TestMethodRemovePackageTundra()
        {
            var database = new LocalDatabase("database", new PackageInstaller());


            EnsureInstalledTestPackages();

            var testPackages = new UnitTestPackageBuilder();
            var testp = testPackages.GetTundra();
            database.UninstallPackage(testp);

            CheckInstallationFiles(testPackages.GetSAE2());
            CheckInstallationFiles(testPackages.GetShadow());

            Assert.IsTrue(true);

        }

        private void EnsureTestPackages()
        {
            var testPackages = new UnitTestPackageBuilder();
            var testp = testPackages.GetSAE1();
            if (!File.Exists(testp.FileName()))
            {
                testPackages.TestMethodCreatePackageSAE1();
            }

            testp = testPackages.GetSAE2();
            if (!File.Exists(testp.FileName()))
            {
                testPackages.TestMethodCreatePackageSAE2();
            }

            testp = testPackages.GetShadow();
            if (!File.Exists(testp.FileName()))
            {
                testPackages.TestMethodCreatePackageShadow();
            }

            testp = testPackages.GetTundra();
            if (!File.Exists(testp.FileName()))
            {
                testPackages.TestMethodCreatePackageTundra();
            }
        }

        private void EnsureInstalledTestPackages()
        {

            if (Directory.Exists("wot_home\\res_mods")) Directory.Delete("wot_home\\res_mods", true);
            Directory.CreateDirectory("wot_home\\res_mods");

            var database = new LocalDatabase("database", new PackageInstaller());
            database.Init("wot_home", "0.9.13", true);

            EnsureTestPackages();

            var testPackages = new UnitTestPackageBuilder();
            var testp = testPackages.GetSAE1();

            database.InstallPackage(File.OpenRead(testp.FileName()));
            Assert.IsTrue(database.GetInstalledPackages().ExistsByName(testp));

            CheckInstallationFiles(testp);

            testp = testPackages.GetSAE2();
            database.InstallPackage(File.OpenRead(testp.FileName()));
            Assert.IsTrue(database.GetInstalledPackages().ExistsByName(testp));

            CheckInstallationFiles(testp);


            testp = testPackages.GetShadow();
            database.InstallPackage(File.OpenRead(testp.FileName()));
            Assert.IsTrue(database.GetInstalledPackages().ExistsByName(testp));

            CheckInstallationFiles(testp);


            testp = testPackages.GetTundra();
            database.InstallPackage(File.OpenRead(testp.FileName()));
            Assert.IsTrue(database.GetInstalledPackages().ExistsByName(testp));

            CheckInstallationFiles(testp);

        }

        private void CheckInstallationFiles(IPackage package)
        {
            var files = Directory.GetFiles(Path.Combine("mods", package.Name), "*", SearchOption.AllDirectories);
            foreach (var orginalFile in files)
            {
                var searchFiles = Directory.GetFiles("wot_home\\res_mods", Path.GetFileName(orginalFile), SearchOption.AllDirectories);
                if (searchFiles.Length != 1)
                    Assert.Fail($"Search for File {Path.GetFileName(orginalFile)} failed.");
                if (new FileInfo(orginalFile).Length != new FileInfo(searchFiles[0]).Length)
                    Assert.Fail($"File {Path.GetFileName(orginalFile)} Length validation failed.");
            }
        }
    }
}
