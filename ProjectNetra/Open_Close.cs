using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ProjectNetra
{
    
    public class Open_Close                                                             // This class handles the opening and closing of apps or other programs
    {

        /*******************  Used for bringing opened apps to foreground  *******************************/
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;

        /*************************************************************************************************/

        private static Dictionary<string,string> browser_path;
        private static Dictionary<string, bool> open_apps;


        public static void Open(string cmd)
        {
            RetrieveSystemWebBrowsers();
            RetrieveAllOpenProcesses();
            switch (cmd)
            {
                case "Open Firefox":
                    if (browser_path["Mozilla Firefox"] == "")                              // Firefox is not installed
                    {
                        Speak_Listen.Speak("Sorry, Firefox is not installed!");
                    }
                    else if (open_apps["firefox"])                                          // Firefox is already open and (in)/(not in) focus
                    {
                        Process[] processes = Process.GetProcessesByName("firefox");
                        foreach (var p in processes)
                        {
                            IntPtr hWnd = p.MainWindowHandle;
                            if (IsIconic(hWnd))
                                ShowWindowAsync(hWnd, SW_RESTORE);
                            SetForegroundWindow(hWnd);
                        }
                        Speak_Listen.Speak("Firefox is opened!");

                    }
                    else                                                                    // Firefox is installed but not open
                    {
                        /*
                        * TODO 1: Report error, if any, while opening the same.
                        */
                        Process p = Process.Start(browser_path["Mozilla Firefox"]);
                        p.WaitForInputIdle();
                        Speak_Listen.Speak("Firefox is Opened!");
                        p.WaitForExit();
                        p.Dispose();
                        Speak_Listen.Speak("Firefox is closed!");
                        Thread.Sleep(100);
                    }
                    break;
                case "Open Chrome":
                    if (browser_path["Google Chrome"] == "")                              // Chrome is not installed
                    {
                        Speak_Listen.Speak("Sorry, Chrome is not installed!");
                    }
                    else if (open_apps["chrome"])                                          // Chrome is already open and (in)/(not in) focus
                    {
                        Process[] processes = Process.GetProcessesByName("chrome");
                        foreach (var p in processes)
                        {
                            IntPtr hWnd = p.MainWindowHandle;
                            if (IsIconic(hWnd))
                                ShowWindowAsync(hWnd, SW_RESTORE);
                            SetForegroundWindow(hWnd);
                        }
                        Speak_Listen.Speak("Chrome is opened!");

                    }
                    else                                                                    // Chrome is installed but not open
                    {
                        /*
                        * TODO 1: Report error, if any, while opening the same.
                        */
                        Process p = Process.Start(browser_path["Google Chrome"]);
                        p.WaitForInputIdle();
                        Speak_Listen.Speak("Chrome is Opened!");
                        p.WaitForExit();
                        p.Dispose();
                        Speak_Listen.Speak("Chrome is closed!");
                        Thread.Sleep(100);
                    }
                    break;
                case "Open Browser":
                case "Open Internet Explorer":
                    if (browser_path["Internet Explorer"] == "")                              // IE is not installed
                    {
                        Speak_Listen.Speak("Sorry, Internet Explorer is not installed!");
                    }
                    else if (open_apps["iexplore"])                                          // IE is already open and (in)/(not in) focus
                    {
                        Process[] processes = Process.GetProcessesByName("iexplore");
                        foreach (var p in processes)
                        {
                            IntPtr hWnd = p.MainWindowHandle;
                            if (IsIconic(hWnd))
                                ShowWindowAsync(hWnd, SW_RESTORE);
                            SetForegroundWindow(hWnd);
                        }
                        Speak_Listen.Speak("Internet Explorer is opened!");

                    }
                    else                                                                    // IE is installed but not open
                    {
                        /*
                        * TODO 1: Report error, if any, while opening the same.
                        */
                        Process p = Process.Start(browser_path["Internet Explorer"]);
                        p.WaitForInputIdle();
                        Speak_Listen.Speak("Internet Explorer is Opened!");
                        p.WaitForExit();
                        p.Dispose();
                        Speak_Listen.Speak("Internet Explorer is closed!");
                        Thread.Sleep(100);
                    }
                    break;
                default:
                    break; 
            }
        }

        private static void Release()
        {

        }

        private static void RetrieveSystemWebBrowsers() {
            browser_path = new Dictionary<string, string>();
            browser_path["Google Chrome"] = "";
            browser_path["Mozilla Firefox"] = "";
            browser_path["Internet Explorer"] = "";
            //browser_path["Microsoft Edge"] = "microsoft-edge:";
            string browser_key = @"SOFTWARE\Clients\StartMenuInternet";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(browser_key))
            {
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        string displayName = sk.GetValue("").ToString();
                        if(browser_path.ContainsKey(displayName))
                        {
                            browser_path[displayName] = sk.OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command").GetValue("").ToString(); 
                        }
                        
                    }
                }
            }
        }
        private static void RetrieveAllOpenProcesses()
        {
            open_apps = new Dictionary<string, bool>();
            open_apps["chrome"] = false;
            open_apps["firefox"] = false;
            open_apps["iexplore"] = false;

            Process[] allprocesses = Process.GetProcesses();
            foreach (var p in allprocesses)
            {
                if (open_apps.ContainsKey(p.ProcessName))
                    open_apps[p.ProcessName] = true;
            }
                        
        }
    }
}
