namespace RDB_WMP
{
    partial class FormPlayer
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
            this.components = new System.ComponentModel.Container();
            this.rdBtnChoice1 = new System.Windows.Forms.RadioButton();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.groupBoxQuestion = new System.Windows.Forms.GroupBox();
            this.rdBtnChoice4 = new System.Windows.Forms.RadioButton();
            this.rdBtnChoice3 = new System.Windows.Forms.RadioButton();
            this.rdBtnChoice2 = new System.Windows.Forms.RadioButton();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.QuestionTimer = new System.Windows.Forms.Timer(this.components);
            this.lblTimeLeft = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnRightAnswer = new System.Windows.Forms.Button();
            this.btnLeaderboard = new System.Windows.Forms.Button();
            this.lblGreeting = new System.Windows.Forms.Label();
            this.groupBoxQuestion.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdBtnChoice1
            // 
            this.rdBtnChoice1.AutoSize = true;
            this.rdBtnChoice1.Location = new System.Drawing.Point(8, 108);
            this.rdBtnChoice1.Name = "rdBtnChoice1";
            this.rdBtnChoice1.Size = new System.Drawing.Size(65, 16);
            this.rdBtnChoice1.TabIndex = 1;
            this.rdBtnChoice1.TabStop = true;
            this.rdBtnChoice1.Text = "Choice1";
            this.rdBtnChoice1.UseVisualStyleBackColor = true;
            // 
            // lblQuestion
            // 
            this.lblQuestion.AutoSize = true;
            this.lblQuestion.Location = new System.Drawing.Point(6, 17);
            this.lblQuestion.MaximumSize = new System.Drawing.Size(475, 0);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(53, 12);
            this.lblQuestion.TabIndex = 0;
            this.lblQuestion.Text = "Question";
            // 
            // groupBoxQuestion
            // 
            this.groupBoxQuestion.Controls.Add(this.rdBtnChoice4);
            this.groupBoxQuestion.Controls.Add(this.rdBtnChoice3);
            this.groupBoxQuestion.Controls.Add(this.rdBtnChoice2);
            this.groupBoxQuestion.Controls.Add(this.rdBtnChoice1);
            this.groupBoxQuestion.Controls.Add(this.lblQuestion);
            this.groupBoxQuestion.Location = new System.Drawing.Point(15, 34);
            this.groupBoxQuestion.Name = "groupBoxQuestion";
            this.groupBoxQuestion.Size = new System.Drawing.Size(487, 327);
            this.groupBoxQuestion.TabIndex = 2;
            this.groupBoxQuestion.TabStop = false;
            // 
            // rdBtnChoice4
            // 
            this.rdBtnChoice4.AutoSize = true;
            this.rdBtnChoice4.Location = new System.Drawing.Point(8, 252);
            this.rdBtnChoice4.Name = "rdBtnChoice4";
            this.rdBtnChoice4.Size = new System.Drawing.Size(65, 16);
            this.rdBtnChoice4.TabIndex = 4;
            this.rdBtnChoice4.TabStop = true;
            this.rdBtnChoice4.Text = "Choice4";
            this.rdBtnChoice4.UseVisualStyleBackColor = true;
            // 
            // rdBtnChoice3
            // 
            this.rdBtnChoice3.AutoSize = true;
            this.rdBtnChoice3.Location = new System.Drawing.Point(8, 205);
            this.rdBtnChoice3.Name = "rdBtnChoice3";
            this.rdBtnChoice3.Size = new System.Drawing.Size(65, 16);
            this.rdBtnChoice3.TabIndex = 3;
            this.rdBtnChoice3.TabStop = true;
            this.rdBtnChoice3.Text = "Choice3";
            this.rdBtnChoice3.UseVisualStyleBackColor = true;
            // 
            // rdBtnChoice2
            // 
            this.rdBtnChoice2.AutoSize = true;
            this.rdBtnChoice2.Location = new System.Drawing.Point(8, 159);
            this.rdBtnChoice2.Name = "rdBtnChoice2";
            this.rdBtnChoice2.Size = new System.Drawing.Size(65, 16);
            this.rdBtnChoice2.TabIndex = 2;
            this.rdBtnChoice2.TabStop = true;
            this.rdBtnChoice2.Text = "Choice2";
            this.rdBtnChoice2.UseVisualStyleBackColor = true;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(132, 373);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // QuestionTimer
            // 
            this.QuestionTimer.Interval = 1000;
            this.QuestionTimer.Tick += new System.EventHandler(this.QuestionTimer_Tick);
            // 
            // lblTimeLeft
            // 
            this.lblTimeLeft.AutoSize = true;
            this.lblTimeLeft.Font = new System.Drawing.Font("Andy", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeLeft.Location = new System.Drawing.Point(379, 15);
            this.lblTimeLeft.Name = "lblTimeLeft";
            this.lblTimeLeft.Size = new System.Drawing.Size(0, 18);
            this.lblTimeLeft.TabIndex = 4;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("Andy", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(32, 50);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(0, 39);
            this.lblResult.TabIndex = 5;
            // 
            // btnRightAnswer
            // 
            this.btnRightAnswer.Location = new System.Drawing.Point(213, 373);
            this.btnRightAnswer.Name = "btnRightAnswer";
            this.btnRightAnswer.Size = new System.Drawing.Size(134, 23);
            this.btnRightAnswer.TabIndex = 6;
            this.btnRightAnswer.Text = "View Right Answers";
            this.btnRightAnswer.UseVisualStyleBackColor = true;
            this.btnRightAnswer.Visible = false;
            this.btnRightAnswer.Click += new System.EventHandler(this.btnRightAnswer_Click);
            // 
            // btnLeaderboard
            // 
            this.btnLeaderboard.Location = new System.Drawing.Point(370, 373);
            this.btnLeaderboard.Name = "btnLeaderboard";
            this.btnLeaderboard.Size = new System.Drawing.Size(111, 23);
            this.btnLeaderboard.TabIndex = 7;
            this.btnLeaderboard.Text = "View Leaderboard";
            this.btnLeaderboard.UseVisualStyleBackColor = true;
            this.btnLeaderboard.Visible = false;
            this.btnLeaderboard.Click += new System.EventHandler(this.btnLeaderboard_Click);
            // 
            // lblGreeting
            // 
            this.lblGreeting.AutoSize = true;
            this.lblGreeting.Font = new System.Drawing.Font("Andy", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGreeting.Location = new System.Drawing.Point(15, 15);
            this.lblGreeting.Name = "lblGreeting";
            this.lblGreeting.Size = new System.Drawing.Size(0, 18);
            this.lblGreeting.TabIndex = 8;
            // 
            // FormPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 411);
            this.Controls.Add(this.lblGreeting);
            this.Controls.Add(this.btnLeaderboard);
            this.Controls.Add(this.btnRightAnswer);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblTimeLeft);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.groupBoxQuestion);
            this.Name = "FormPlayer";
            this.Text = "SET Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPlayer_FormClosing);
            this.Load += new System.EventHandler(this.FormPlayer_Load);
            this.groupBoxQuestion.ResumeLayout(false);
            this.groupBoxQuestion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdBtnChoice1;
        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.GroupBox groupBoxQuestion;
        private System.Windows.Forms.RadioButton rdBtnChoice2;
        private System.Windows.Forms.RadioButton rdBtnChoice4;
        private System.Windows.Forms.RadioButton rdBtnChoice3;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Timer QuestionTimer;
        private System.Windows.Forms.Label lblTimeLeft;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnRightAnswer;
        private System.Windows.Forms.Button btnLeaderboard;
        private System.Windows.Forms.Label lblGreeting;
    }
}

