﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace PluginAPI
{
    /// <summary>
    /// This class contains many useful methods for usage in your plugin
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Executes a cmd command (non threaded).
        /// </summary>
        /// <param name="command"></param>
        public static void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                 new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                Console.WriteLine(objException); // Log the exception
            }
        }

        /// <summary>
        /// Executes a cmd command (threaded).
        /// </summary>
        /// <param name="command"></param>
        public static void ExecuteCommandAsync(string command)
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteCommandSync));
                //Make the thread as background thread.
                objThread.IsBackground = true;
                //Set the Priority of the thread.
                objThread.Priority = ThreadPriority.AboveNormal;
                //Start the thread.
                objThread.Start(command);
            }
            catch (ThreadStartException objException)
            {
                Console.WriteLine(objException);
                // Log the exception
            }
            catch (ThreadAbortException objException)
            {
                // Log the exception
                Console.WriteLine(objException);
            }
            catch (Exception objException)
            {
                // Log the exception
                Console.WriteLine(objException);
            }
        }


        /// <summary>
        /// Writes an error message to console.
        /// </summary>
        /// <param name="message"></param>
        public static void ErrorWriteLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a notification message to console.
        /// </summary>
        /// <param name="message"></param>
        public static void NotifyWriteLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Asks user a simple yes/no statement.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool YesNoStatement(String message)
        {
            Console.WriteLine(message + " Y/N:");
            string Answer = Console.ReadLine();
            if (Answer.ToLower().Contains("y") || Answer.ToLower().Contains("yes"))
                return true;
            else if (Answer.ToLower().Contains("n") || Answer.ToLower().Contains("no"))
                return false;
            else
                Console.WriteLine("Unknown Answer");
            return false;
        }

        /// <summary>
        /// Asks user for text input.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string TextInput(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }

        /// <summary>
        /// The same as TextInput
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string SameLineTextInput(string message)
        {
            Console.Write(message + ": ");
            return Console.ReadLine();
        }

        /// <summary>
        /// returns true if supplied file or directory exists.
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static bool FileOrDirectoryExists(string Path)
        {
            return (Directory.Exists(Path) || File.Exists(Path));
        }

        /// <summary>
        /// Returns true if current user account is admin.
        /// </summary>
        /// <returns></returns>
        public static bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            System.Security.Principal.WindowsIdentity user = null;
            try
            {
                //get the currently logged in user
                user = System.Security.Principal.WindowsIdentity.GetCurrent();
                System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(user);
                isAdmin = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
#pragma warning disable 0168
            catch (UnauthorizedAccessException ex)
#pragma warning restore 0168
            {
                isAdmin = false;
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
                isAdmin = false;
            }
            finally
            {
                if (user != null)
                    user.Dispose();
            }
            return isAdmin;
        }

        /// <summary>
        /// Skips a line in the console
        /// </summary>
        private static void SkipALine()
        {
            Console.WriteLine("");
        }

        /// <summary>
        /// Returns whether or not a class is real
        /// </summary>
        /// <param name="testType"></param>
        /// <returns></returns>
        public static bool IsRealClass(Type testType)
        {
            return !(testType.IsAbstract || testType.IsGenericTypeDefinition || testType.IsInterface);
        }

        /// <summary>
        /// Returns all types in the current AppDomain implementing the interface or inheriting the type. 
        /// </summary>
        public static IEnumerable<Type> TypesImplementingInterface(Type desiredType)
        {
            return AppDomain
                   .CurrentDomain
                   .GetAssemblies()
                   .SelectMany(assembly => assembly.GetTypes())
                   .Where(type => desiredType.IsAssignableFrom(type));
        }

        /// <summary>
        /// Restarts the program
        /// </summary>
        public static void RestartProgram()
        {
            System.Diagnostics.Process.Start(Assembly.GetEntryAssembly().Location);
            // Closes the current process
            Environment.Exit(0);
        }



        /// <summary>
        /// Writes a notice next time the program launches.
        /// </summary>
        /// <param name="notice"></param>
        public static void WriteNotice(string notice)
        {



            string noticefile = "notice.nf";
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);



            File.AppendAllText(exeDir + "/" + noticefile, notice + Environment.NewLine);

        }


        /// <summary>
        /// Sleeps for the specified amount of time in seconds.
        /// </summary>
        /// <param name="seconds"></param>
        public static void Wait(float seconds)
        {
            int waittime = Convert.ToInt32(seconds * 1000F);
            Thread.Sleep((waittime));
        }


        private const string _CapsLockMessage = " CAPS LOCK";

        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <param name="mask">a <c>char</c> representing your choice of console mask</param>
        /// <returns>the string the user typed in</returns>
        public static string ReadLineMasked(char mask = '*')
        {
            // Taken from http://stackoverflow.com/a/19770778/486660
            var consoleLine = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            bool isDone;
            bool isAlreadyLocked;
            bool isCapsLockOn;
            int cursorLeft;
            int cursorTop;
            ConsoleColor originalForegroundColor;

            isDone = false;
            isAlreadyLocked = Console.CapsLock;

            while (isDone == false)
            {
                isCapsLockOn = Console.CapsLock;
                if (isCapsLockOn != isAlreadyLocked)
                {
                    if (isCapsLockOn)
                    {
                        cursorLeft = Console.CursorLeft;
                        cursorTop = Console.CursorTop;
                        originalForegroundColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("{0}", _CapsLockMessage);
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        Console.ForegroundColor = originalForegroundColor;
                    }
                    else
                    {
                        cursorLeft = Console.CursorLeft;
                        cursorTop = Console.CursorTop;
                        Console.Write("{0}", string.Empty.PadRight(_CapsLockMessage.Length));
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                    }
                    isAlreadyLocked = isCapsLockOn;
                }

                if (Console.KeyAvailable)
                {
                    keyInfo = Console.ReadKey(intercept: true);

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        isDone = true;
                        continue;
                    }

                    if (!char.IsControl(keyInfo.KeyChar))
                    {
                        consoleLine.Append(keyInfo.KeyChar);
                        Console.Write(mask);
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace && consoleLine.Length > 0)
                    {
                        consoleLine.Remove(consoleLine.Length - 1, 1);

                        if (Console.CursorLeft == 0)
                        {
                            Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                            Console.Write(' ');
                            Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                        }
                        else
                        {
                            Console.Write("\b \b");
                        }
                    }

                    if (isCapsLockOn)
                    {
                        cursorLeft = Console.CursorLeft;
                        cursorTop = Console.CursorTop;
                        originalForegroundColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("{0}", _CapsLockMessage);
                        Console.CursorLeft = cursorLeft;
                        Console.CursorTop = cursorTop;
                        Console.ForegroundColor = originalForegroundColor;
                    }
                }
            }

            Console.WriteLine();

            return consoleLine.ToString();
        }






    }
}
