﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCLauncher
{
    internal class JavaModHelper
    {
        public static void Start(string instName, string clientPath)
        {
            int modNum = 0;
            List<string> modList = new List<string>();

            string filepath = $"{Globals.dataPath}\\instance\\{instName}\\jarmods\\";
            DirectoryInfo d = new DirectoryInfo(filepath);

            foreach (var file in d.GetFiles("*.jar"))
            {
                Console.WriteLine(file.Name);
                modNum++;
                modList.Add(file.FullName);
            }

            foreach (var file in d.GetFiles("*.zip"))
            {
                Console.WriteLine(file.Name);
                modNum++;
                modList.Add(file.FullName);
            }

            if (modNum > 0)
            {
                string toHash = "";
                Logger.Info("[ModHelper]", "Mods detected!");

                //get patchhash first
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(clientPath))
                    {
                        var hash = md5.ComputeHash(stream);
                        toHash += BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant() + ";";
                    }
                }
                foreach (var mod in modList)
                {
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(mod))
                        {
                            var hash = md5.ComputeHash(stream);
                            toHash += BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant() + ";";
                        }
                    }
                }
                toHash += "CodexIpsa";
                Console.WriteLine(toHash);
                string patchHash = "";
                using (var md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(toHash);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    patchHash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
                    Console.WriteLine(patchHash);
                }

                if (!File.Exists($"{Globals.dataPath}\\instance\\{instName}\\jarmods\\patch\\{patchHash}.jar"))
                {
                    //create patch TODO: THIS IS SHIT AND NEEDS TO BE IMPROVED
                    if (Directory.Exists($"{Globals.dataPath}\\instance\\{instName}\\jarmods\\temp\\"))
                        Directory.Delete($"{Globals.dataPath}\\instance\\{instName}\\jarmods\\temp\\", true);


                    ZipFile.ExtractToDirectory(clientPath, $"{Globals.dataPath}\\instance\\{instName}\\jarmods\\temp\\full\\");

                    int modCount = 0;
                    foreach (var mod in modList)
                    {
                        ZipFile.ExtractToDirectory(mod, $"{Globals.dataPath}\\instance\\{instName}\\jarmods\\temp\\{modCount}\\");
                        modCount++;
                    }

                    for (int i = 0; i < modCount; i++)
                    {
                        string sourcePath = $"{Globals.dataPath}\\instance\\{instName}\\jarmods\\temp\\{i}\\";

                        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                        {
                            Directory.CreateDirectory(dirPath.Replace(sourcePath, $"{Globals.dataPath}\\instance\\{instName}\\jarmods\\temp\\full\\"));
                        }

                        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                        {
                            File.Copy(newPath, newPath.Replace(sourcePath, $"{Globals.dataPath}\\instance\\{instName}\\jarmods\\temp\\full\\"), true);
                        }
                    }

                    Directory.Delete($"{Globals.dataPath}\\instance\\{instName}\\jarmods\\temp\\full\\META-INF\\", true);
                    Directory.CreateDirectory($"{Globals.dataPath}\\instance\\{instName}\\jarmods\\patch\\");
                    File.Delete($"{Globals.dataPath}\\instance\\{instName}\\jarmods\\patch\\patch.jar");
                    ZipFile.CreateFromDirectory($"{Globals.dataPath}\\instance\\{instName}\\jarmods\\temp\\full\\", $"{Globals.dataPath}\\instance\\{instName}\\jarmods\\patch\\{patchHash}.jar");
                    Directory.Delete($"{Globals.dataPath}\\instance\\{instName}\\jarmods\\temp\\", true);

                    Logger.Info("[ModHelper]", "Created patched jar!");
                }

                LaunchJava.launchClientPath = $"{Globals.dataPath}\\instance\\{instName}\\jarmods\\patch\\{patchHash}.jar";

                string aa = File.ReadAllText($"{Globals.dataPath}\\instance\\{instName}\\jarmods\\index.cfg");
                if (aa.Contains("\"forge\":true"))
                {
                    LaunchJava.launchProxy += "-Dhttp.nonProxyHosts=codex-ipsa.dejvoss.cz -Dminecraft.applet.TargetDirectory={gameDir} -Dfml.core.libraries.mirror=http://codex-ipsa.dejvoss.cz/MCL-Data/launcher/forgelib/%s ";
                    Logger.Info("[ModHelper]", "Forge tweaks on!");
                }
            }
        }
    }
}
