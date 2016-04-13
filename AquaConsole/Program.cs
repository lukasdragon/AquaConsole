﻿using System;
using System.Linq;
using PluginAPI;
using System.IO;

using AquaConsole.Managers;

namespace AquaConsole
{

    class Program
    {
        public static Boolean quitNow = false;

        public static string ProgramVersion = "AquaDark Console [Version 1.3.0100]";

        static void Main(string[] args)
        {
            Setup();
            String RootCommand;
            String command;
            String Argument;


            while (!quitNow)
            {
                Console.WriteLine("");
                Console.Write(Environment.CurrentDirectory + ">");
                RootCommand = Console.ReadLine();
                command = RootCommand.Split(' ').First();
                Argument = RootCommand.Remove(command.IndexOf(command), command.Length);




                CommandManager.RunCommand(command, Argument);


            }
        }

        private static void Setup()
        {
            if (!Utility.FileOrDirectoryExists("/plugins/"))
                Directory.CreateDirectory("/plugins");

            

            PluginManager.loadPlugins("plugins");
            CommandManager.LoadCommands();
            NoticeManager.ReadNotice();
            Environment.CurrentDirectory = "C:/";

            if (!Utility.IsUserAdministrator())
                Utility.ErrorWriteLine("Warning, Admin priveleges not detected, not all commands will work!");
            Console.Title = "AquaDark Console";
            Console.WriteLine(ProgramVersion);
            Console.WriteLine("<c> 2016 AquaDark Corporation. All rights reserved.");

            CommandManager.HelpText.Sort();
        }





    }
}
