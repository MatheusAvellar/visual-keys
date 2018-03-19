using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace VisualKeys {
    static class Program {
        static IDictionary<string, Control> labels;

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _hookID = SetHook(_proc);
            VisualKeyWindow window = new VisualKeyWindow();
            labels = new Dictionary<string, Control>();
            foreach(Control c in window.Controls.OfType<Control>().Where(x => x is Label)) {
                labels.Add(c.Name, c);
            }
            Application.Run(window);
            UnhookWindowsHookEx(_hookID);
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
 
        private static IntPtr SetHook(LowLevelKeyboardProc proc) {
            using(Process curProcess = Process.GetCurrentProcess())
            using(ProcessModule curModule = curProcess.MainModule) {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            // KEYDOWN event
            if(nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) {
                int vkCode = Marshal.ReadInt32(lParam);

                if(labels.ContainsKey("key" + vkCode)) {
                    labels["key" + vkCode].Enabled = true;
                    labels["key" + vkCode].BackColor = System.Drawing.Color.Yellow;

                    UpdateLockKeys((Keys)vkCode);
                } else {
                    Console.WriteLine(vkCode);
                }
            // KEYUP event
            } else if(nCode >= 0 && wParam == (IntPtr)WM_KEYUP) {
                int vkCode = Marshal.ReadInt32(lParam);
                if(labels.ContainsKey("key" + vkCode)) {
                    labels["key" + vkCode].Enabled = false;
                    labels["key" + vkCode.ToString()].BackColor = System.Drawing.Color.Transparent;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void UpdateLockKeys(Keys key) {
            if(key == Keys.NumLock) {
                if(!Control.IsKeyLocked(Keys.NumLock))
                    labels["numLockOn"].ForeColor = System.Drawing.Color.CornflowerBlue;
                else
                    labels["numLockOn"].ForeColor = System.Drawing.Color.LightGray;
            } else if(key == Keys.CapsLock) {
                if(!Control.IsKeyLocked(Keys.CapsLock))
                    labels["capsLockOn"].ForeColor = System.Drawing.Color.CornflowerBlue;
                else
                    labels["capsLockOn"].ForeColor = System.Drawing.Color.LightGray;
            } else if(key == Keys.Scroll) {
                if(!Control.IsKeyLocked((Keys)145))
                    labels["scrollLockOn"].ForeColor = System.Drawing.Color.CornflowerBlue;
                else
                    labels["scrollLockOn"].ForeColor = System.Drawing.Color.LightGray;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
