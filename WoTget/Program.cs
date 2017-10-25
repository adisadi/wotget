using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using WoTget.Core.Commands;

namespace WoTget.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            //.AddInMemoryCollection(new MySettings{WoTDirectory="C:\\Games\\World_Of_Tanks"})
            .AddJsonFile("settings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var settings = new MySettings();
            configuration.Bind(settings);

            if (string.IsNullOrEmpty(settings.WoTDirectory)){
                settings.WoTDirectory= "C:\\Games\\World_Of_Tanks";
            }

            Application.InitializeInstance("key.json",settings.WoTDirectory);
            var cmdLineapp = new CommandLineApplication();
            RootCommand.Configure(cmdLineapp);
            cmdLineapp.Execute(args);
        }
    }

    public class MySettings
    {
        public string WoTDirectory { get; set; }
    }
}
