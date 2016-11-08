namespace FreeMove
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_From = new System.Windows.Forms.TextBox();
            this.button_BrowseFrom = new System.Windows.Forms.Button();
            this.button_BrowseTo = new System.Windows.Forms.Button();
            this.textBox_To = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button_Move = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Move From:";
            // 
            // textBox_From
            // 
            this.textBox_From.Location = new System.Drawing.Point(83, 13);
            this.textBox_From.Name = "textBox_From";
            this.textBox_From.Size = new System.Drawing.Size(383, 20);
            this.textBox_From.TabIndex = 1;
            // 
            // button_BrowseFrom
            // 
            this.button_BrowseFrom.Location = new System.Drawing.Point(472, 13);
            this.button_BrowseFrom.Name = "button_BrowseFrom";
            this.button_BrowseFrom.Size = new System.Drawing.Size(75, 23);
            this.button_BrowseFrom.TabIndex = 2;
            this.button_BrowseFrom.Text = "Browse...";
            this.button_BrowseFrom.UseVisualStyleBackColor = true;
            this.button_BrowseFrom.Click += new System.EventHandler(this.button_BrowseFrom_Click);
            // 
            // button_BrowseTo
            // 
            this.button_BrowseTo.Location = new System.Drawing.Point(472, 39);
            this.button_BrowseTo.Name = "button_BrowseTo";
            this.button_BrowseTo.Size = new System.Drawing.Size(75, 23);
            this.button_BrowseTo.TabIndex = 4;
            this.button_BrowseTo.Text = "Browse...";
            this.button_BrowseTo.UseVisualStyleBackColor = true;
            this.button_BrowseTo.Click += new System.EventHandler(this.button_BrowseTo_Click);
            // 
            // textBox_To
            // 
            this.textBox_To.Location = new System.Drawing.Point(83, 39);
            this.textBox_To.Name = "textBox_To";
            this.textBox_To.Size = new System.Drawing.Size(383, 20);
            this.textBox_To.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "To:";
            // 
            // button_Move
            // 
            this.button_Move.Location = new System.Drawing.Point(472, 95);
            this.button_Move.Name = "button_Move";
            this.button_Move.Size = new System.Drawing.Size(75, 23);
            this.button_Move.TabIndex = 6;
            this.button_Move.Text = "Move";
            this.button_Move.UseVisualStyleBackColor = true;
            this.button_Move.Click += new System.EventHandler(this.button_Move_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(16, 65);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(154, 17);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Set original folder to hidden";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 130);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button_Move);
            this.Controls.Add(this.button_BrowseTo);
            this.Controls.Add(this.textBox_To);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_BrowseFrom);
            this.Controls.Add(this.textBox_From);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Free Move";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_From;
        private System.Windows.Forms.Button button_BrowseFrom;
        private System.Windows.Forms.Button button_BrowseTo;
        private System.Windows.Forms.TextBox textBox_To;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button_Move;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

