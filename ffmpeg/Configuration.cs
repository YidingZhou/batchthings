using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoClient
{
    public partial class Configuration : Form
    {
        public Configuration()
        {
            InitializeComponent();
            tbAccount.Text = Settings.storageAccount;
            tbKey.Text = Settings.storageKey;
            

            foreach (batchaccount a in Settings.accounts)
                cbBatchAccount.Items.Add(a.account);

            cbBatchAccount.Text = Settings.batchAccount;
            tbBatchKey.Text = Settings.batchKey;
            tbBatchEndPoint.Text = Settings.batchEndpoint;
            
            updateaccount();
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            Settings.storageAccount = tbAccount.Text;
            Settings.storageKey = tbKey.Text;
            Settings.batchAccount = cbBatchAccount.Text;
            Settings.batchKey = tbBatchKey.Text;
            Settings.batchEndpoint = tbBatchEndPoint.Text;

            Close();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateaccount();
        }

        private void updateaccount()
        {
            foreach (batchaccount a in Settings.accounts)
            {
                if (a.account == cbBatchAccount.Text)
                {
                    tbBatchKey.Text = a.key;
                    tbBatchEndPoint.Text = a.url;
                    break;
                }
            }
        }
    }
}
