using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace VClipboardHelper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            DoWork();

            SetLastActiveWindow();
        }

        private static void SetLastActiveWindow()
        {
            IntPtr lastWindowHandle = GetWindow(Process.GetCurrentProcess().MainWindowHandle, (uint)GetWindow_Cmd.GW_HWNDNEXT);
            while (true)
            {
                IntPtr temp = GetParent(lastWindowHandle);
                if (temp.Equals(IntPtr.Zero)) break;
                lastWindowHandle = temp;
            }
            SetForegroundWindow(lastWindowHandle);
        }

        private static void DoWork()
        {
            var mainInput = Clipboard.GetText();

            if (IsStackTrace(mainInput))
            {
                var update = mainInput.Replace(" at ", Environment.NewLine + " at ");
                Clipboard.SetText(update);
                return;
            }

            // list to CSV
            if (mainInput.Contains(Environment.NewLine))
            {
                Clipboard.SetText(mainInput.Replace(Environment.NewLine, ","));
                return;
            }

            // CSV to single-quoted CSV
            if (mainInput.Contains(",") && !mainInput.StartsWith("'") && !mainInput.EndsWith("'"))
            {
                Clipboard.SetText(string.Concat("'", mainInput.Replace(",", "','"), "'"));
                return;
            }
        }

        private static bool IsStackTrace(string mainInput)
        {
            return mainInput.Contains("Exception") 
                || mainInput.Contains("exception") 
                || mainInput.Contains("stack trace");
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);



    }
}
