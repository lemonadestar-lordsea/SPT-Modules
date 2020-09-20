using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using SPTarkov.Launcher.Controllers;

namespace SPTarkov.Launcher
{
    public partial class Program : Application
    {
        private string GetGamePath()
        {
            string gamePath = Environment.CurrentDirectory;

            try
            {
                using (StreamReader sr = new StreamReader($"{Environment.CurrentDirectory}\\launcher.config.json"))
                {
                    string data = sr.ReadToEnd();

                    data = data.Replace("\r", "").Replace("\n", "").Replace('\"', ' ');

                    var regex = Regex.Match(data, @"GamePath\s+:\s+([\w\d\\][^,]+)\s+,");

                    if (regex.Success)
                    {
                        gamePath = regex.Groups[1].Value;
                    }
                }
            }
            catch (Exception)
            {
            }

            return gamePath;
        }
        private void Application_Startup(object s, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            //I'm not sure if you want application specific exception handling. AppDomain should handle them all AFAIK. You had something similar before, so I'm just adding this in. (might cause duplicate messageboxes though)
            Current.DispatcherUnhandledException += (sender, args) => HandleException(args.Exception);

            // load assemblies from EFT's managed directory
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => AssemblyResolveEvent(sender, args, GetGamePath());

            // run launcher
            SPTarkovLauncherMainWindow LauncherWindow = new SPTarkovLauncherMainWindow();
            LauncherWindow.ShowDialog();
		}

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleException(exception);
            }
            else
            {
                HandleException(new Exception("Unknown Exception!"));
            }
        }

        private static void HandleException(Exception exception)
        {
            var text = $"Exception Message:{exception.Message}{Environment.NewLine}StackTrace:{exception.StackTrace}";
			LogManager.Instance.Error(text);

            MessageBox.Show(text, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static Assembly AssemblyResolveEvent(object sender, ResolveEventArgs args, string gamePath)
        {
            string assembly = new AssemblyName(args.Name).Name;
            string filename = Path.Combine(gamePath, $"EscapeFromTarkov_Data/Managed/{assembly}.dll");

            // resources are embedded inside assembly
            if (filename.Contains("resources"))
            {
                return null;
            }

            return Assembly.LoadFrom(filename);
        }
    }
}
