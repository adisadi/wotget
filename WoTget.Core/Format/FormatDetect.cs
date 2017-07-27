using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WoTget.Core.Format
{
    public class FormatDetect
    {
        public enum PackageFormat
        {
            NoFormat,
            WotHomeRoot, //res_mods 
            ResModRoot, // 0.9.18
            VersionRoot, //Scripts,....

            ZipFileWotHomeRoot, //res_mods
            ZipFileResModRoot, // 0.9.18
            ZipFileVersionRoot, //Scripts,....

            WotModHomeRoot, // mods
            WotModModsRoot, //0.9.18

            WotMod, //wotmod File
            WotModZipHomeRoot, // mods
            WotModZipModsRoot //0.9.18
        }

        public static PackageFormat Analize(IEnumerable<string> files)
        {
            PackageFormat value = PackageFormat.NoFormat;

            if (files.Count() == 0) throw new ArgumentException("No files!");

            if (files.Count() == 1)
            {
                var file = files.First();
                if (file.ToLower().EndsWith(Constants.WotModExtension))
                {
                    return PackageFormat.WotMod;
                }
                else if (file.ToLower().EndsWith(".zip"))
                {
                    using (var archive = new ZipArchive(File.OpenRead(file), ZipArchiveMode.Read, false))
                    {
                        //search wotmod file
                        if (archive.Entries.Any(e => e.Name.EndsWith(Constants.WotModExtension)))
                        {
                            PackageFormat tempFormat = PackageFormat.NoFormat;
                            foreach (var entry in archive.Entries.Where(e => e.Name != string.Empty))
                            {
                                var entryFileName = entry.FullName;
                                if (checkWoModHomeRoot(entryFileName))
                                {
                                    if (tempFormat != PackageFormat.NoFormat && tempFormat != PackageFormat.WotModZipHomeRoot)
                                        throw new ArgumentException();
                                    tempFormat = PackageFormat.WotModZipHomeRoot;
                                    continue;
                                }

                                if (checkWoModModsRoot(entryFileName, false))
                                {
                                    if (tempFormat != PackageFormat.NoFormat && tempFormat != PackageFormat.WotModZipModsRoot)
                                        throw new ArgumentException();
                                    tempFormat = PackageFormat.WotModZipHomeRoot;
                                    continue;
                                }
                            }
                            value = tempFormat;
                        }
                        else
                        {
                            PackageFormat tempFormat = PackageFormat.NoFormat;
                            foreach (var entry in archive.Entries.Where(e => e.Name != string.Empty))
                            {
                                var entryFileName = entry.FullName;

                                //Check for wot home root
                                if (checkWotHomeRoot(entryFileName))
                                {
                                    if (tempFormat != PackageFormat.NoFormat && tempFormat != PackageFormat.ZipFileWotHomeRoot)
                                        throw new ArgumentException();
                                    tempFormat = PackageFormat.ZipFileWotHomeRoot;
                                    continue;
                                }

                                //check for resmods Root           
                                if (checkResModsRoot(entryFileName, false))
                                {
                                    if (tempFormat != PackageFormat.NoFormat && tempFormat != PackageFormat.ZipFileResModRoot)
                                        throw new ArgumentException();
                                    tempFormat = PackageFormat.ZipFileResModRoot;
                                    continue;
                                }
                                //check version root folders
                                if (checkVersionRoot(entryFileName))
                                {
                                    if (tempFormat != PackageFormat.NoFormat && tempFormat != PackageFormat.ZipFileVersionRoot)
                                        throw new ArgumentException();
                                    tempFormat = PackageFormat.ZipFileVersionRoot;
                                    continue;
                                }


                            }
                            value = tempFormat;
                        }

                    }
                }
            }
            else
            {
                //search wotmod file
                if (files.Any(e => e.EndsWith(Constants.WotModExtension)))
                {
                    PackageFormat tempFormat = PackageFormat.NoFormat;
                    foreach (var file in files)
                    {
                        if (checkWoModHomeRoot(file))
                        {
                            if (tempFormat != PackageFormat.NoFormat && tempFormat != PackageFormat.WotModHomeRoot)
                                throw new ArgumentException();
                            tempFormat = PackageFormat.WotModHomeRoot;
                            continue;
                        }

                        if (checkWoModModsRoot(file))
                        {
                            if (tempFormat != PackageFormat.NoFormat && tempFormat != PackageFormat.WotModModsRoot)
                                throw new ArgumentException();
                            tempFormat = PackageFormat.WotModHomeRoot;
                            continue;
                        }
                    }
                    value = tempFormat;

                }
                else
                {
                    PackageFormat tempFormat = PackageFormat.NoFormat;
                    foreach (var file in files)
                    {
                        //Check for wot home root
                        if (checkWotHomeRoot(file))
                        {
                            if (tempFormat != PackageFormat.NoFormat && tempFormat != PackageFormat.WotHomeRoot)
                                throw new ArgumentException();
                            tempFormat = PackageFormat.WotHomeRoot;
                            continue;
                        }

                        //check for resmods Root
                        if (checkResModsRoot(file))
                        {
                            if (tempFormat != PackageFormat.NoFormat && tempFormat != PackageFormat.ResModRoot)
                                throw new ArgumentException();
                            tempFormat = PackageFormat.ResModRoot;
                            continue;
                        }

                        //check version root folders
                        if (checkVersionRoot(file))
                        {
                            if (tempFormat != PackageFormat.NoFormat && tempFormat != PackageFormat.VersionRoot)
                                throw new ArgumentException();
                            tempFormat = PackageFormat.VersionRoot;
                            continue;
                        }

                    }
                    value = tempFormat;
                }
            }

            return value;
        }

        private static bool checkVersionRoot(string file)
        {
            var matchVersionRoot = Regex.Match(file, "audiowww|gui|maps|objects|scripts|system");
            return matchVersionRoot.Success;
        }

        private static bool checkResModsRoot(string file, bool backSlash = true)
        {
            var matchString = backSlash ? "(?<=\\\\)([0-9]{1,2}\\.?)+" : "(?<=/)([0-9]{1,2}\\.?)+";

            var matchResModsRoot = Regex.Match(file, matchString);
            return matchResModsRoot.Success;
        }

        private static bool checkWotHomeRoot(string file)
        {
            var matchHomeRoot = Regex.Match(file, "res_mods");
            return matchHomeRoot.Success;
        }

        private static bool checkWoModHomeRoot(string file)
        {
            var matchHomeRoot = Regex.Match(file, "mods");
            return matchHomeRoot.Success;
        }

        private static bool checkWoModModsRoot(string file, bool backSlash = true)
        {
            var matchString = backSlash ? "(?<=\\\\)([0-9]{1,2}\\.?)+" : "(?<=/)([0-9]{1,2}\\.?)+";

            var matchResModsRoot = Regex.Match(file, matchString);
            return matchResModsRoot.Success;
        }
    }
}
