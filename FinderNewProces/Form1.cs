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

namespace FinderNewProces
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GetProcess();
        }
        
        private void GetProcess()
        {
            listBox1.Items.Clear();
            foreach (Process p in Process.GetProcesses())
                listBox1.Items.Add(p.ToString() +" "+ p.ProcessName);
        }

        private void Sorted()
        {
            
        }
    }
}
