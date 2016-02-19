namespace RDB_WMP
{
    partial class FormAskName
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
            this.lblIPInfo = new System.Windows.Forms.Label();
            this.txtBoxIP = new System.Windows.Forms.TextBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtBoxName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblIPInfo
            // 
            this.lblIPInfo.AutoSize = true;
            this.lblIPInfo.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIPInfo.Location = new System.Drawing.Point(189, 150);
            this.lblIPInfo.Name = "lblIPInfo";
            this.lblIPInfo.Size = new System.Drawing.Size(86, 15);
            this.lblIPInfo.TabIndex = 12;
            this.lblIPInfo.Text = "(e.g. 192.10.10.10)";
            // 
            // txtBoxIP
            // 
            this.txtBoxIP.Location = new System.Drawing.Point(17, 147);
            this.txtBoxIP.Name = "txtBoxIP";
            this.txtBoxIP.Size = new System.Drawing.Size(170, 21);
            this.txtBoxIP.TabIndex = 11;
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Font = new System.Drawing.Font("Andy", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIP.Location = new System.Drawing.Point(13, 121);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(101, 23);
            this.lblIP.TabIndex = 10;
            this.lblIP.Text = "IP Address:";
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Andy", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(93, 191);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(104, 33);
            this.btnConnect.TabIndex = 9;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnName_Click);
            // 
            // txtBoxName
            // 
            this.txtBoxName.Location = new System.Drawing.Point(11, 70);
            this.txtBoxName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtBoxName.Name = "txtBoxName";
            this.txtBoxName.Size = new System.Drawing.Size(176, 21);
            this.txtBoxName.TabIndex = 8;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Andy", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(9, 44);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(200, 23);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Please enter your name:";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("SimSun", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblError.Location = new System.Drawing.Point(17, 166);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 11);
            this.lblError.TabIndex = 13;
            // 
            // FormAskName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 259);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.lblIPInfo);
            this.Controls.Add(this.txtBoxIP);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtBoxName);
            this.Controls.Add(this.lblName);
            this.Name = "FormAskName";
            this.Text = "SET Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIPInfo;
        private System.Windows.Forms.TextBox txtBoxIP;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtBoxName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblError;
    }
}