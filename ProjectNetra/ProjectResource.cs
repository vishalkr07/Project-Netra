﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNetra
{
    public class ProjectResource
    {
        public static string[] AssistantCommandList =
        {
            "Open Firefox",
            "Open Chrome",
            "Open Internet Explorer",
            "Open Music",
            "Open Mail",
            "Open File Manager",
            "Open File Explorer",
            "Open Explorer",
            "Open PDF Reader",
            "Open Word",
            "Open Settings",
            "Open Calculator",
            "Increase Volume",
            "Decrease Volume",
            "Close Firefox",
            "Close Chrome",
            "Close Internet Explorer",
            "Close Music",
            "Close Mail",
            "Close File Manager",
            "Close File Explorer",
            "Close Explorer",
            "Close PDF Reader",
            "Close Word",
            "Close Settings",
            "Close Calculator"
        };

        public static string[] MediaPlayerCommand =
        {
            "Play",
            "Pause",
            "Stop",
            "Next",
            "Previous",
            "Repeat On",
            "Repeat Off",
            "Open List"
        };

        public static string[] CalculatorCommand =
        {
            "point",
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
            "plus",
            "minus",
            "divide",
            "product",
            "equal",
            "delete",
            "clear",
            "sine",
            "cos",
            "tan",
            "expo",
            "square",
            "power",
            "mod",
            "log",
            "root",
            "store",
            "MemC",
            "load"
        };

        public static string[] FileManagerCommand =
        {
            "Back",
            "Next",
            "Up",
            "Down",
            "Files",
            "Folders",
            "Refresh",
            "Read Out",
            "Controls",
            "Filter",
            "New Folder",
            "New File",
            "Delete",
            "Rename",
            "Open"
            
        };

        public static string[] GetNoGrammer(int l, int r)
        {
            string[] s = new string[(r-l+1)];
            for (int i = l; i <= r; i++)
            {
                s[i - l] = i.ToString();
            }
            return s;
        }
    }
}
