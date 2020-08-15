namespace UpdateRaceCard
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConfJV = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnGetJVData = new System.Windows.Forms.Button();
            this.axJVLink1 = new AxJVDTLabLib.AxJVLink();
            this.tmrDownload = new System.Windows.Forms.Timer(this.components);
            this.prgDownload = new System.Windows.Forms.ProgressBar();
            this.prgJVRead = new System.Windows.Forms.ProgressBar();
            this.rtbData = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axJVLink1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuConfig});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(534, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "設定(&C)";
            // 
            // mnuConfig
            // 
            this.mnuConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuConfJV});
            this.mnuConfig.Name = "mnuConfig";
            this.mnuConfig.Size = new System.Drawing.Size(58, 20);
            this.mnuConfig.Text = "設定(&C)";
            // 
            // mnuConfJV
            // 
            this.mnuConfJV.Name = "mnuConfJV";
            this.mnuConfJV.Size = new System.Drawing.Size(155, 22);
            this.mnuConfJV.Text = "JLinkの設定(&J)...";
            this.mnuConfJV.Click += new System.EventHandler(this.mnuConfJV_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(294, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "1 更新する出馬表を格納しているフォルダを選択してください。";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBox1.Location = new System.Drawing.Point(60, 56);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(456, 23);
            this.textBox1.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(15, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(45, 25);
            this.button1.TabIndex = 5;
            this.button1.Text = "選択";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnGetJVData
            // 
            this.btnGetJVData.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetJVData.Location = new System.Drawing.Point(15, 94);
            this.btnGetJVData.Name = "btnGetJVData";
            this.btnGetJVData.Size = new System.Drawing.Size(75, 44);
            this.btnGetJVData.TabIndex = 7;
            this.btnGetJVData.Text = "データ取得";
            this.btnGetJVData.UseVisualStyleBackColor = true;
            this.btnGetJVData.Click += new System.EventHandler(this.btnGetJVData_Click);
            // 
            // axJVLink1
            // 
            this.axJVLink1.Enabled = true;
            this.axJVLink1.Location = new System.Drawing.Point(434, 94);
            this.axJVLink1.Name = "axJVLink1";
            this.axJVLink1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axJVLink1.OcxState")));
            this.axJVLink1.Size = new System.Drawing.Size(26, 23);
            this.axJVLink1.TabIndex = 8;
            // 
            // prgDownload
            // 
            this.prgDownload.Location = new System.Drawing.Point(97, 96);
            this.prgDownload.Name = "prgDownload";
            this.prgDownload.Size = new System.Drawing.Size(419, 21);
            this.prgDownload.TabIndex = 9;
            // 
            // prgJVRead
            // 
            this.prgJVRead.Location = new System.Drawing.Point(97, 117);
            this.prgJVRead.Name = "prgJVRead";
            this.prgJVRead.Size = new System.Drawing.Size(419, 21);
            this.prgJVRead.TabIndex = 10;
            // 
            // rtbData
            // 
            this.rtbData.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rtbData.Location = new System.Drawing.Point(15, 153);
            this.rtbData.Name = "rtbData";
            this.rtbData.Size = new System.Drawing.Size(506, 81);
            this.rtbData.TabIndex = 11;
            this.rtbData.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 253);
            this.Controls.Add(this.rtbData);
            this.Controls.Add(this.prgJVRead);
            this.Controls.Add(this.prgDownload);
            this.Controls.Add(this.axJVLink1);
            this.Controls.Add(this.btnGetJVData);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "JV-Link出馬表更新ツール";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axJVLink1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuConfig;
        private System.Windows.Forms.ToolStripMenuItem mnuConfJV;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button btnGetJVData;
        public AxJVDTLabLib.AxJVLink axJVLink1;
        public System.Windows.Forms.Timer tmrDownload;
        public System.Windows.Forms.ProgressBar prgDownload;
        public System.Windows.Forms.ProgressBar prgJVRead;
        public System.Windows.Forms.RichTextBox rtbData;
    }
}

