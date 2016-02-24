using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using WoTget.Core.Repositories;
using System.Linq;
using System.Collections.Generic;
using WoTget.Core.Authoring;

namespace WoTget.Core.Test
{
    [TestClass]
    public class UnitTestLocalRepository
    {
        private const string localDatabaseDir = "local-database";
        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            if (!Directory.Exists(localDatabaseDir))
            {
                Directory.CreateDirectory(localDatabaseDir);
            }
        }

        private Package GetTestPackage1()
        {
            return new Package { Name = "test", Version = "1.0" };
        }
        private Package GetTestPackage11()
        {
            return new Package { Name = "test", Version = "1.1" };
        }

        [TestMethod]
        public void TestMethodEmptyRepo()
        {
            var repo = new LocalRepository(localDatabaseDir);
            var packages = repo.GetPackages();
            Assert.IsTrue(packages.Count() == 0);
            packages = repo.GetPackages(false);
            Assert.IsTrue(packages.Count() == 0);

            packages = repo.GetPackages("test");
            Assert.IsTrue(packages.Count() == 0);
            packages = repo.GetPackages("test", false);
            Assert.IsTrue(packages.Count() == 0);

            packages = repo.GetPackages(new List<string> { "test" });
            Assert.IsTrue(packages.Count() == 0);
            packages = repo.GetPackages(new List<string> { "test" }, false);
            Assert.IsTrue(packages.Count() == 0);

            try
            {
                var stream = repo.GetPackage(GetTestPackage1());
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }

            try
            {
                repo.RemovePackage(GetTestPackage1());
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }


        }

        [TestMethod]
        [DeploymentItem("Ressources\\modpack\\res_mods\\", "Ressources\\res_mods")]
        public void TestMethodAddPackage()
        {
            var repo = new LocalRepository(localDatabaseDir);
            repo.AddPackage(GetTestPackage1(), Directory.GetFiles("Ressources\\res_mods", "*", SearchOption.AllDirectories));
            repo.AddPackage(GetTestPackage11(), Directory.GetFiles("Ressources\\res_mods", "*", SearchOption.AllDirectories));

            Assert.IsTrue(File.Exists(Path.Combine(localDatabaseDir, "test\\1.0.0\\" + Constants.ManifestFileName)));
            Assert.IsTrue(File.Exists(Path.Combine(localDatabaseDir, "test\\1.0.0\\" + GetTestPackage1().FileName())));
            Assert.IsTrue(File.Exists(Path.Combine(localDatabaseDir, "test\\1.1.0\\" + Constants.ManifestFileName)));
            Assert.IsTrue(File.Exists(Path.Combine(localDatabaseDir, "test\\1.1.0\\" + GetTestPackage11().FileName())));
        }

        [TestMethod]
        [DeploymentItem("Ressources\\modpack\\res_mods\\", "Ressources\\res_mods")]
        public void TestMethodRemovePackage()
        {
            TestMethodAddPackage();

            var repo = new LocalRepository(localDatabaseDir);
            repo.RemovePackage(GetTestPackage1());

            Assert.IsTrue(!File.Exists(Path.Combine(localDatabaseDir, "test\\1.0.0\\" + Constants.ManifestFileName)));
            Assert.IsTrue(!File.Exists(Path.Combine(localDatabaseDir, "test\\1.0.0\\" + GetTestPackage1().FileName())));
        }

        [TestMethod]
        [DeploymentItem("Ressources\\modpack\\res_mods\\", "Ressources\\res_mods")]
        public void TestMethodGetPackages()
        {
            TestMethodAddPackage();

            var repo = new LocalRepository(localDatabaseDir);
            var packages=  repo.GetPackages(true).ToList();
            Assert.IsTrue(packages.Count() == 1 && packages[0].Version=="1.1");


            packages = repo.GetPackages(false).ToList();
            Assert.IsTrue(packages.Count() == 2);
        }
    }
}
