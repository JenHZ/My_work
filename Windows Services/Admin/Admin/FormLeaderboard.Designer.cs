namespace Admin
{
    partial class FormLeaderboard
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
            this.lblLeaderboards = new System.Windows.Forms.Label();
            this.dataGridViewLeaderboard = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLeaderboard)).BeginInit();
            this.SuspendLayout();
            // 
            // lblLeaderboards
            // 
            this.lblLeaderboards.AutoSize = true;
            this.lblLeaderboards.Font = new System.Drawing.Font("Andy", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeaderboards.Location = new System.Drawing.Point(23, 8);
            this.lblLeaderboards.Name = "lblLeaderboards";
            this.lblLeaderboards.Size = new System.Drawing.Size(159, 30);
            this.lblLeaderboards.TabIndex = 4;
            this.lblLeaderboards.Text = "Leaderboards:";
            // 
            // dataGridViewLeaderboard
            // 
            this.dataGridViewLeaderboard.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewLeaderboard.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewLeaderboard.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridViewLeaderboard.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewLeaderboard.Location = new System.Drawing.Point(22, 36);
            this.dataGridViewLeaderboard.Name = "dataGridViewLeaderboard";
            this.dataGridViewLeaderboard.ReadOnly = true;
            this.dataGridViewLeaderboard.RowHeadersVisible = false;
            this.dataGridViewLeaderboard.RowHeadersWidth = 20;
            this.dataGridViewLeaderboard.RowTemplate.Height = 23;
            this.dataGridViewLeaderboard.Size = new System.Drawing.Size(425, 346);
            this.dataGridViewLeaderboard.TabIndex = 3;
            // 
            // FormLeaderboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 432);
            this.Controls.Add(this.lblLeaderboards);
            this.Controls.Add(this.dataGridViewLeaderboard);
            this.Name = "FormLeaderboard";
            this.Text = "SET Test Leaderboard";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLeaderboard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLeaderboards;
        private System.Windows.Forms.DataGridView dataGridViewLeaderboard;
    }
}