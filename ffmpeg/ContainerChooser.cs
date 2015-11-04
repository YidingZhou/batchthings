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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DemoClient
{
    public partial class ContainerChooser : Form
    {
        public string ContainerChoosen = string.Empty;
        public ContainerChooser()
        {
            InitializeComponent();
            bgContainerFinder.WorkerSupportsCancellation = true;
            bgContainerFinder.RunWorkerAsync();
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            ContainerChoosen = cbContainer.Text;
            Close();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            bgContainerFinder.CancelAsync();
            Close();
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void bgContainerFinder_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            IEnumerable<string> containers = StorageHelper.ListContainers();

            while (true)
            {
                if (cbContainer.IsHandleCreated)
                {
                    cbContainer.Invoke((MethodInvoker)delegate() { cbContainer.Items.AddRange(containers.ToArray()); });
                    break;
                }
                Thread.Sleep(1000);
            }

        }
    }
}
