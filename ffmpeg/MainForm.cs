using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace DemoClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            backgroundWorker.RunWorkerAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbInput.Text = Settings.inputContainer;
            cbOutput.Text = Settings.outputContainer;
            cbResource.Text = Settings.resourceContainer;

            bgContainerFinder.WorkerSupportsCancellation = true;
            bgContainerFinder.RunWorkerAsync();
        }

        private void bConfigure_Click(object sender, EventArgs e)
        {
            DialogResult res = (new Configuration()).ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
                bgContainerFinder.RunWorkerAsync();
        }

        private void bChooseInput_Click(object sender, EventArgs e)
        {
            ContainerChooser cc = new ContainerChooser();
            DialogResult res = cc.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
                cbInput.Text = cc.ContainerChoosen; 
        }

        private void bChooseResource_Click(object sender, EventArgs e)
        {
            ContainerChooser cc = new ContainerChooser();
            DialogResult res = cc.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
                cbResource.Text = cc.ContainerChoosen;
        }

        private void bChooseOutput_Click(object sender, EventArgs e)
        {
            ContainerChooser cc = new ContainerChooser();
            DialogResult res = cc.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
                cbOutput.Text = cc.ContainerChoosen;
        }

        private void bSubmit_Click(object sender, EventArgs e)
        {
            Settings.inputContainer = cbInput.Text;
            Settings.resourceContainer = cbResource.Text;
            Settings.outputContainer = cbOutput.Text;
            
            ThreadPool.QueueUserWorkItem(x =>
            {
                BatchServiceClient.Submit();
            });
        }

        private void bTerminate_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(x => BatchServiceClient.Terminate());
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (BatchServiceClient.IsRunning())
            {
                DialogResult res = MessageBox.Show("Do you want to terminate job before quit?", "Info", MessageBoxButtons.YesNoCancel);
                if (res == System.Windows.Forms.DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    BatchServiceClient.Terminate();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    string log = BatchServiceClient.FetchLog();
                    if (string.IsNullOrEmpty(log))
                    {
                        Thread.Sleep(1000);
                        TaskbarManager.Instance.SetProgressValue(BatchServiceClient.progress, 100);
                        pbJobProgress.Invoke((MethodInvoker)delegate() { pbJobProgress.Value = BatchServiceClient.progress; });
                        if (!BatchServiceClient.IsRunning())
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                    }
                    else
                    {
                        tbLogs.Invoke((MethodInvoker)delegate() { tbLogs.AppendText(log + "\n"); });
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        private void bgContainerFinder_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Settings.storageAccount == null || Settings.storageKey == null)
                return;

            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                IEnumerable<string> containers = StorageHelper.ListContainers();

                while (true)
                {
                    if (cbInput.IsHandleCreated)
                    {
                        cbInput.Invoke((MethodInvoker)delegate() { cbInput.Items.AddRange(containers.ToArray()); });
                        break;
                    }
                    Thread.Sleep(1000);
                }

                while (true)
                {
                    if (cbOutput.IsHandleCreated)
                    {
                        cbOutput.Invoke((MethodInvoker)delegate() { cbOutput.Items.AddRange(containers.ToArray()); });
                        break;
                    }
                    Thread.Sleep(1000);
                }

                while (true)
                {
                    if (cbResource.IsHandleCreated)
                    {
                        cbResource.Invoke((MethodInvoker)delegate() { cbResource.Items.AddRange(containers.ToArray()); });
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch { }
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbCLI_TextChanged(object sender, EventArgs e)
        {
            Settings.commands = cbCLI.Text;
        }

        private void bDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete all your workitems?", "Delete", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                ThreadPool.QueueUserWorkItem(x => BatchServiceClient.Delete());
        }

        private void bCreatePool_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Wipe out and recreate the pool?", "Create Pool", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                ThreadPool.QueueUserWorkItem(x => BatchServiceClient.ReCreatePool());
        }
    }
}
