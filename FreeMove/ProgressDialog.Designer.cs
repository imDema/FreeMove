// FreeMove -- Move directories without breaking shortcuts or installations 
//    Copyright(C) 2020  Luca De Martini

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.

namespace FreeMove
{
    partial class ProgressDialog
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label_Message = new System.Windows.Forms.Label();
            this.label_Progress = new System.Windows.Forms.Label();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 35);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(260, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 0;
            // 
            // label_Message
            // 
            this.label_Message.AutoSize = true;
            this.label_Message.Location = new System.Drawing.Point(12, 9);
            this.label_Message.Name = "label_Message";
            this.label_Message.Size = new System.Drawing.Size(131, 13);
            this.label_Message.TabIndex = 1;
            this.label_Message.Text = "Moving files, please wait...";
            // 
            // label_Progress
            // 
            this.label_Progress.Location = new System.Drawing.Point(149, 9);
            this.label_Progress.Name = "label_Progress";
            this.label_Progress.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_Progress.Size = new System.Drawing.Size(123, 13);
            this.label_Progress.TabIndex = 2;
            this.label_Progress.Text = "123/321";
            this.label_Progress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button_Cancel
            // 
            this.button_Cancel.Location = new System.Drawing.Point(197, 64);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 3;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            // 
            // MoveDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 94);
            this.ControlBox = false;
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.label_Progress);
            this.Controls.Add(this.label_Message);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MoveDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Moving Files...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label_Message;
        private System.Windows.Forms.Label label_Progress;
        private System.Windows.Forms.Button button_Cancel;
    }
}