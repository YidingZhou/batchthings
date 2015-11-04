namespace DemoClient
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bSubmit = new System.Windows.Forms.Button();
            this.bTerminate = new System.Windows.Forms.Button();
            this.bConfigure = new System.Windows.Forms.Button();
            this.pbJobProgress = new System.Windows.Forms.ProgressBar();
            this.tbLogs = new System.Windows.Forms.TextBox();
            this.bChooseInput = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbInput = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbResource = new System.Windows.Forms.ComboBox();
            this.bChooseResource = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbOutput = new System.Windows.Forms.ComboBox();
            this.bChooseOutput = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbCLI = new System.Windows.Forms.ComboBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.bgContainerFinder = new System.ComponentModel.BackgroundWorker();
            this.bClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.bDelete = new System.Windows.Forms.Button();
            this.bCreatePool = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // bSubmit
            // 
            this.bSubmit.Location = new System.Drawing.Point(499, 242);
            this.bSubmit.Name = "bSubmit";
            this.bSubmit.Size = new System.Drawing.Size(75, 23);
            this.bSubmit.TabIndex = 0;
            this.bSubmit.Text = "&Submit";
            this.bSubmit.UseVisualStyleBackColor = true;
            this.bSubmit.Click += new System.EventHandler(this.bSubmit_Click);
            // 
            // bTerminate
            // 
            this.bTerminate.Location = new System.Drawing.Point(418, 242);
            this.bTerminate.Name = "bTerminate";
            this.bTerminate.Size = new System.Drawing.Size(75, 23);
            this.bTerminate.TabIndex = 1;
            this.bTerminate.Text = "&Terminate";
            this.bTerminate.UseVisualStyleBackColor = true;
            this.bTerminate.Click += new System.EventHandler(this.bTerminate_Click);
            // 
            // bConfigure
            // 
            this.bConfigure.Location = new System.Drawing.Point(12, 242);
            this.bConfigure.Name = "bConfigure";
            this.bConfigure.Size = new System.Drawing.Size(75, 23);
            this.bConfigure.TabIndex = 2;
            this.bConfigure.Text = "C&onfigure...";
            this.bConfigure.UseVisualStyleBackColor = true;
            this.bConfigure.Click += new System.EventHandler(this.bConfigure_Click);
            // 
            // pbJobProgress
            // 
            this.pbJobProgress.Location = new System.Drawing.Point(12, 287);
            this.pbJobProgress.Name = "pbJobProgress";
            this.pbJobProgress.Size = new System.Drawing.Size(561, 23);
            this.pbJobProgress.Step = 1;
            this.pbJobProgress.TabIndex = 3;
            // 
            // tbLogs
            // 
            this.tbLogs.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLogs.Location = new System.Drawing.Point(12, 316);
            this.tbLogs.Multiline = true;
            this.tbLogs.Name = "tbLogs";
            this.tbLogs.ReadOnly = true;
            this.tbLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLogs.Size = new System.Drawing.Size(561, 449);
            this.tbLogs.TabIndex = 4;
            // 
            // bChooseInput
            // 
            this.bChooseInput.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bChooseInput.Location = new System.Drawing.Point(530, 16);
            this.bChooseInput.Name = "bChooseInput";
            this.bChooseInput.Size = new System.Drawing.Size(25, 23);
            this.bChooseInput.TabIndex = 6;
            this.bChooseInput.Text = "...";
            this.bChooseInput.UseVisualStyleBackColor = true;
            this.bChooseInput.Visible = false;
            this.bChooseInput.Click += new System.EventHandler(this.bChooseInput_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbInput);
            this.groupBox1.Controls.Add(this.bChooseInput);
            this.groupBox1.Location = new System.Drawing.Point(13, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(561, 51);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input";
            // 
            // cbInput
            // 
            this.cbInput.FormattingEnabled = true;
            this.cbInput.Location = new System.Drawing.Point(6, 18);
            this.cbInput.Name = "cbInput";
            this.cbInput.Size = new System.Drawing.Size(549, 21);
            this.cbInput.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbResource);
            this.groupBox2.Controls.Add(this.bChooseResource);
            this.groupBox2.Location = new System.Drawing.Point(13, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(561, 51);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Resource";
            // 
            // cbResource
            // 
            this.cbResource.FormattingEnabled = true;
            this.cbResource.Location = new System.Drawing.Point(6, 18);
            this.cbResource.Name = "cbResource";
            this.cbResource.Size = new System.Drawing.Size(549, 21);
            this.cbResource.TabIndex = 8;
            // 
            // bChooseResource
            // 
            this.bChooseResource.Location = new System.Drawing.Point(530, 16);
            this.bChooseResource.Name = "bChooseResource";
            this.bChooseResource.Size = new System.Drawing.Size(25, 23);
            this.bChooseResource.TabIndex = 6;
            this.bChooseResource.Text = "...";
            this.bChooseResource.UseVisualStyleBackColor = true;
            this.bChooseResource.Visible = false;
            this.bChooseResource.Click += new System.EventHandler(this.bChooseResource_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbOutput);
            this.groupBox3.Controls.Add(this.bChooseOutput);
            this.groupBox3.Location = new System.Drawing.Point(13, 125);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(561, 51);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output";
            // 
            // cbOutput
            // 
            this.cbOutput.FormattingEnabled = true;
            this.cbOutput.Location = new System.Drawing.Point(6, 18);
            this.cbOutput.Name = "cbOutput";
            this.cbOutput.Size = new System.Drawing.Size(549, 21);
            this.cbOutput.TabIndex = 8;
            // 
            // bChooseOutput
            // 
            this.bChooseOutput.Location = new System.Drawing.Point(530, 16);
            this.bChooseOutput.Name = "bChooseOutput";
            this.bChooseOutput.Size = new System.Drawing.Size(25, 23);
            this.bChooseOutput.TabIndex = 6;
            this.bChooseOutput.Text = "...";
            this.bChooseOutput.UseVisualStyleBackColor = true;
            this.bChooseOutput.Visible = false;
            this.bChooseOutput.Click += new System.EventHandler(this.bChooseOutput_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbCLI);
            this.groupBox4.Location = new System.Drawing.Point(13, 182);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(561, 51);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Commands";
            this.groupBox4.Visible = false;
            // 
            // cbCLI
            // 
            this.cbCLI.FormattingEnabled = true;
            this.cbCLI.Location = new System.Drawing.Point(6, 20);
            this.cbCLI.Name = "cbCLI";
            this.cbCLI.Size = new System.Drawing.Size(549, 21);
            this.cbCLI.TabIndex = 0;
            this.cbCLI.Visible = false;
            this.cbCLI.TextChanged += new System.EventHandler(this.cbCLI_TextChanged);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // bgContainerFinder
            // 
            this.bgContainerFinder.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgContainerFinder_DoWork);
            // 
            // bClose
            // 
            this.bClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bClose.Location = new System.Drawing.Point(337, 242);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(75, 23);
            this.bClose.TabIndex = 12;
            this.bClose.Text = "&Close";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(13, 274);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(561, 2);
            this.label1.TabIndex = 13;
            // 
            // bDelete
            // 
            this.bDelete.Location = new System.Drawing.Point(93, 242);
            this.bDelete.Name = "bDelete";
            this.bDelete.Size = new System.Drawing.Size(89, 23);
            this.bDelete.TabIndex = 14;
            this.bDelete.Text = "&Delete Jobs";
            this.bDelete.UseVisualStyleBackColor = true;
            this.bDelete.Click += new System.EventHandler(this.bDelete_Click);
            // 
            // bCreatePool
            // 
            this.bCreatePool.Location = new System.Drawing.Point(188, 242);
            this.bCreatePool.Name = "bCreatePool";
            this.bCreatePool.Size = new System.Drawing.Size(75, 23);
            this.bCreatePool.TabIndex = 15;
            this.bCreatePool.Text = "Create &Pool";
            this.bCreatePool.UseVisualStyleBackColor = true;
            this.bCreatePool.Click += new System.EventHandler(this.bCreatePool_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bClose;
            this.ClientSize = new System.Drawing.Size(586, 777);
            this.Controls.Add(this.bCreatePool);
            this.Controls.Add(this.bDelete);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bClose);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbLogs);
            this.Controls.Add(this.pbJobProgress);
            this.Controls.Add(this.bConfigure);
            this.Controls.Add(this.bTerminate);
            this.Controls.Add(this.bSubmit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Demo Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bSubmit;
        private System.Windows.Forms.Button bTerminate;
        private System.Windows.Forms.Button bConfigure;
        private System.Windows.Forms.ProgressBar pbJobProgress;
        private System.Windows.Forms.TextBox tbLogs;
        private System.Windows.Forms.Button bChooseInput;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bChooseResource;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bChooseOutput;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ComboBox cbCLI;
        private System.Windows.Forms.ComboBox cbInput;
        private System.Windows.Forms.ComboBox cbResource;
        private System.Windows.Forms.ComboBox cbOutput;
        private System.ComponentModel.BackgroundWorker bgContainerFinder;
        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bDelete;
        private System.Windows.Forms.Button bCreatePool;
    }
}

