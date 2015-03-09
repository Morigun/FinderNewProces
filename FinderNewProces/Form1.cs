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
using ROOT.CIMV2.Win32;
using WMI.Win32;


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
            //GetProcess();
            listBox1.Items.Clear();
            GetProcessOnWin32_Process();
            ProcessWatcher procWatcher = new ProcessWatcher();
            procWatcher.ProcessCreated += new ProcessEventHandler(procWatcher_ProcessCreated);
            procWatcher.ProcessDeleted += new ProcessEventHandler(procWatcher_ProcessDeleted);
            procWatcher.ProcessModified += new ProcessEventHandler(procWatcher_ProcessModified);
            while (true)
            {
                procWatcher.WaitForNextEvent(); //ожидаем следующее событие
            }
        }

        static void procWatcher_ProcessCreated(Win32_Process process)
        {
            MessageBox.Show("\nCreated\n " + process.Name + " " + process.ProcessId);
        }

        static void procWatcher_ProcessDeleted(Win32_Process proc)
        {
            MessageBox.Show("\nDeleted\n");
        }

        static void procWatcher_ProcessModified(Win32_Process proc)
        {
            MessageBox.Show("\nModified\n");
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
                    Proc.InvokeMethod("GetOwner", (object[])OwnerInfo);

                    listBox1.Items.Add(String.Format("{0}: {1}", Path.GetFileName(ExecutablePath), OwnerInfo[0]));
                }
            }
        }

        private void Sorted()
        {
            
        }
    }


}

namespace WMI.Win32
{
    public delegate void ProcessEventHandler(Win32_Process proc);

  public class ProcessWatcher: ManagementEventWatcher
  {
    // События процесса
    public event ProcessEventHandler ProcessCreated;
    public event ProcessEventHandler ProcessDeleted;
    public event ProcessEventHandler ProcessModified;

   // WMI WQL запросы
   static readonly string WMI_OPER_EVENT_QUERY = @"SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'"; // запрос на получения списка всех процессов
   static readonly string WMI_OPER_EVENT_QUERY_WITH_PROC = WMI_OPER_EVENT_QUERY + " and TargetInstance.Name = '{0}'"; // запрос на получение конкретного процесса по имени запущеного .exe файла

   public ProcessWatcher()
   {
     Init(string.Empty);
   }

   public ProcessWatcher(string processName)
   {
     Init(processName);
   }
   private void Init(string processName)
   {
     this.Query.QueryLanguage = "WQL";
     if (string.IsNullOrEmpty(processName))
     {
       this.Query.QueryString = WMI_OPER_EVENT_QUERY;
     }
     else
     {
       this.Query.QueryString =
       string.Format(WMI_OPER_EVENT_QUERY_WITH_PROC, processName);
     }

     this.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
   }

   private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
   {
     string eventType = e.NewEvent.ClassPath.ClassName;
     Win32_Process proc = new
     Win32_Process(e.NewEvent["TargetInstance"] as ManagementBaseObject);

     // определяем какое событие произошло
     switch (eventType)
     {
       case "__InstanceCreationEvent":
         if (ProcessCreated != null)
           ProcessCreated(proc); 
         break;
       case "__InstanceDeletionEvent":
         if (ProcessDeleted != null)
           ProcessDeleted(proc); 
         break;
       case "__InstanceModificationEvent":
         if (ProcessModified != null)
           ProcessModified(proc); break;
     }
   }
  }
}