using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;

namespace Marketer
{
    public partial class Form1 : Form
    {
        public string ProcessName = "Tibia";
        public bool IsReady = false;

        #region DLL Imports
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, bool bInheritHandle, UInt32 dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesWritten);
        [DllImport("user32.dll")]
        public static extern int SetActiveWindow(int hwnd);
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);
        [DllImport("user32.dll")]
        static extern bool SetKeyboardState(byte[] lpKeyState);
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        

        #endregion

        #region Declares
        IntPtr hProc = IntPtr.Zero;
        private const int SW_SHOWMAXIMIZED = 3;
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;

        const uint dwAllAccess = 0x1F0FFF;
        // byte[] bBuff = new byte[10];
        // bool bFound = false;
        bool running = false;
        Thread marketThread;
        Process p;
        #endregion


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            button1.Visible = false;
            button2.Visible = false;
            information_label.Visible = false;
            progressBar1.Visible = false;
            inject();
            information_label.AutoSize = false;
            information_label.TextAlign = ContentAlignment.MiddleCenter;
            information_label.Dock = DockStyle.None;
            information_label.Left = 10;
            information_label.Width = this.Width - 10;

        }

        private void inject()
        {
            if (Process.GetProcessesByName(ProcessName).Length == 1)
            {
                hProc = OpenProcess(dwAllAccess, true, (uint)Process.GetProcessesByName(ProcessName)[0].Id);
                IsReady = true;
                this.Text = "Marketer (Running)";
                button1.Visible = false;
                error_label.Text = "";
                button2.Visible = true;
                progressBar1.Visible = true;

                //information label
                information_label.Text = "This will start over the process.";
                information_label.Visible = true;

            }
            else
            {
                this.Text = "Marketer (Stopped)";
                error_label.Text = "Cannot inject. Process not opened.";

                if (IsReady == false)
                {
                    button1.Visible = true;
                    return;
                }
            }
        }
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }
        public static void RightMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, xpos, ypos, 0, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            inject();
        }
        private void forceActivity()
        {
            if (running)
            {

            }
            else
            {


            }
        }
        private void run()
        {
            if (connectionCheck())
            {
                if (running)
                {
                    this.BackColor = Color.LightGray;
                    running = false;
                    button2.Text = "Start Marketting.";
                    turnOff();
                }
                else
                {
                    information_label.Text = "Force Stop Marketting.";
                    button2.Text = "Force Stop.";
                    this.BackColor = Color.Red;
                    running = true;
                    turnOn();
                }
            }
        }
       


        private void turnOn()
        {
            int i = 0;

            string[] worlds = { "Antica", "Next World" };
            BringWindowToFront();
            Thread.Sleep(1000);
            LeftMouseClick(2008, 840);
            Thread.Sleep(1000);
            information_label.Text = "Logging in.";
            SendKeys.SendWait("marketaccount");
            Thread.Sleep(500);
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait(""); //Password here
            Thread.Sleep(500);
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(2000);
            SendKeys.SendWait("{ENTER}");
            information_label.Text = "Logged In! Currently on world: " + worlds[i];
            Thread thread = new Thread(doMarket);
            thread.Start();

        }
        private void doMarket()
        {
            while (running){
                Console.WriteLine("x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);
            }
        }
        private void turnOff()
        {
            information_label.Text = "Logging off.";
            BringWindowToFront();
            Thread.Sleep(500);
            LeftMouseClick(3823, 356);
            Thread.Sleep(500);
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(500);
            SendKeys.SendWait("{ESC}");



            information_label.Text = "Logged off, you're safe to restart it again. =)";
        }
        private void rotate()
        {
            SendKeys.Send("^(Q)");
            SendKeys.Send("{DOWN}");
            SendKeys.Send("{~}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            run();
        }
        private bool connectionCheck()
        {
            if (Process.GetProcessesByName(ProcessName).Length == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void BringWindowToFront()
        {
            Process[] arrProcesses = Process.GetProcessesByName("Tibia");
            if (arrProcesses.Length > 0)
            {
                ShowWindow(arrProcesses[0].MainWindowHandle, SW_SHOWMAXIMIZED);
                IntPtr ipHwnd = arrProcesses[0].MainWindowHandle;
                Thread.Sleep(100);
                SetForegroundWindow(ipHwnd);

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public Array worldnames()
        {

            string[] worlds = {"Antica", "Next World"};
            return worlds.ToArray();
        }
    }
}
