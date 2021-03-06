﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerAPI
{
    class Program
    {
        public static Process MCserver;
        public static string lastMessage;
        static void Main(string[] args)
        {
            var conf = loadConfig("config.txt");

            MCserver = getMinecraftServer(conf.path, conf.args);
            MCserver.Start();

            //start web server
            WebApi.start(conf.url);

            var readStream = MCserver.StandardOutput;

            //redirect output to Console
            while (true)
            {
                string line = readStream.ReadLine();
                if (!String.IsNullOrEmpty(line))
                {
                    lastMessage = line;
                    Console.WriteLine(line);
                }

                Thread.Sleep(100);
            }
        }


        private static Process getMinecraftServer(string path, string args)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = @"-jar " + path,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };

            return proc;
        }

        private static Config loadConfig(string path)
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }

        class Config
        {
            public string path { get; set; }
            public string args { get; set; }
            public string url { get; set; }
        }
    }
}
