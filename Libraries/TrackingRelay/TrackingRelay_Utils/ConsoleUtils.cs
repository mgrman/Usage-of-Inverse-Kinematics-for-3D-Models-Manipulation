using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TrackingRelay_Utils
{
    public static class ConsoleUtils
    {
        [DllImport("kernel32.dll")]
        private static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        private static extern Boolean FreeConsole();


        public static void ShowConsole()
        {
            AllocConsole();
        }
        public static void HideConsole()
        {
            FreeConsole();
        }
    }
}
