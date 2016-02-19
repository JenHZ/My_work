namespace RDB_WMP
{
    partial class FormRightAnswers
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
            this.listBoxQA = new System.Windows.Forms.ListBox();
            this.lblSubmition = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBoxQA
            // 
            this.listBoxQA.FormattingEnabled = true;
            this.listBoxQA.ItemHeight = 12;
            this.listBoxQA.Location = new System.Drawing.Point(13, 37);
            this.listBoxQA.Name = "listBoxQA";
            this.listBoxQA.Size = new System.Drawing.Size(458, 412);
            this.listBoxQA.TabIndex = 0;
            // 
            // lblSubmition
            // 
            this.lblSubmition.AutoSize = true;
            this.lblSubmition.Font = new System.Drawing.Font("Andy", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubmition.Location = new System.Drawing.Point(13, 7);
            this.lblSubmition.Name = "lblSubmition";
            this.lblSubmition.Size = new System.Drawing.Size(122, 30);
            this.lblSubmition.TabIndex = 1;
            this.lblSubmition.Text = "Submition:";
            // 
            // FormRightAnswers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 453);
            this.Controls.Add(this.lblSubmition);
            this.Controls.Add(this.listBoxQA);
            this.Name = "FormRightAnswers";
            this.Text = "SET Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxQA;
        private System.Windows.Forms.Label lblSubmition;
    }
}