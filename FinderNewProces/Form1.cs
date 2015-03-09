using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;
using System.Management.Instrumentation;
using System.IO;
using FinderNewProces;


namespace FinderNewProces
{
    public partial class Form1 : Form
    {
        UnicodeEncoding uniEncoding = new UnicodeEncoding();
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            //GetProcess();
            listBox1.Items.Clear();
            GetProcessOnWin32_Process();
        }       
        
        /*Наименование процесса*/
        private void GetProcess()
        {            
            foreach (Process p in Process.GetProcesses())
                listBox1.Items.Add(p.ProcessName + " ");
        }

        /*Данные о пользователе и процессе*/
        private void GetProcessOnWin32_Process()
        {
            /*CopyPaste from http://stackoverflow.com/questions/300449/how-do-you-get-the-username-of-the-owner-of-a-process */

            ManagementObjectSearcher Processes = new ManagementObjectSearcher("SELECT * FROM Win32_Process");
            
            foreach (ManagementObject Proc in Processes.Get())
            {
                if (Proc["ExecutablePath"] != null)
                {
                    string ExecutablePath = Proc["ExecutablePath"].ToString();

                    string[] OwnerInfo = new string[2];
                    string[] SidInfo = new string[50];
                    Proc.InvokeMethod("GetOwner", (object[])OwnerInfo);
                    Proc.InvokeMethod("GetOwnerSid", (object[])SidInfo);                   

                    listBox1.Items.Add(String.Format("{0}: {1} {2} {3}",
                        Path.GetFileName(ExecutablePath), OwnerInfo[0], SidInfo[0], Proc.Path));
                }
            }
        }

        private void Sorted()
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            using(FileStream fs = new FileStream(Application.StartupPath+@"\TrustProcess.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fs.Write(uniEncoding.GetBytes(listBox1.SelectedItem.ToString()),
                    0, uniEncoding.GetByteCount(listBox1.SelectedItem.ToString()));
            }
        }
    }


}