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
        const int dataArraySize = 100;
        string sQuery;
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            //GetProcess();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            listBox5.Items.Clear();
            listBox6.Items.Clear();
            sQuery = "";
            GetListTrustedProcess();
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

            ManagementObjectSearcher Processes = new ManagementObjectSearcher("SELECT * FROM Win32_Process where " + sQuery);            
            foreach (ManagementObject Proc in Processes.Get())
            {
                if (Proc["ExecutablePath"] != null)
                {
                    string ExecutablePath = Proc["ExecutablePath"].ToString();                    
                    string[] OwnerInfo = new string[2];
                    string[] SidInfo = new string[50];
                    Proc.InvokeMethod("GetOwner", (object[])OwnerInfo);
                    Proc.InvokeMethod("GetOwnerSid", (object[])SidInfo);            

                    listBox1.Items.Add(String.Format("{0}", Path.GetFileName(ExecutablePath)));
                    listBox2.Items.Add(Proc.GetPropertyValue("Name").ToString());
                    listBox3.Items.Add(Proc.GetPropertyValue("Handle").ToString());
                    listBox4.Items.Add(Proc.GetPropertyValue("OSName").ToString());
                    listBox5.Items.Add(Proc.GetPropertyValue("CommandLine").ToString());
                    listBox6.Items.Add(Proc.GetPropertyValue("ExecutablePath").ToString());
                }
            }
        }

        private void Sorted()
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //File.WriteAllText(Application.StartupPath + @"\TrustProcess.txt", listBox1.SelectedItem.ToString());                        
            using (StreamWriter file = new StreamWriter(Application.StartupPath + @"\TrustProcess.txt",true))
            {
                file.WriteLine(listBox1.SelectedItem.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !(timer1.Enabled);
        }

        /*Извращения но в связи с неизвестностью WQL ключевого слова in пока буду действовать так*/
        private void GetListTrustedProcess()
        {
            int iFs = 0;
            try
            {
                string[] lines = File.ReadAllLines(Application.StartupPath + @"\TrustProcess.txt");
                foreach (string s in lines)
                {
                    if (iFs == 0)
                        sQuery += "name != '" + s + "'";
                    else
                        sQuery += " and " + "name != '" + s + "'";
                    iFs++;
                }
            }
            catch(SystemException ex)
            {
                sQuery = "1=1";
                /*Заглушка для 1 запуска*/
            }
        }
    }
}