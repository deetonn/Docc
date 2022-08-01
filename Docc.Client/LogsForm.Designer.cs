namespace Docc.Client
{
    partial class LogsForm
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
            this.LogFileTextBox = new System.Windows.Forms.TextBox();
            this.CopyButton = new System.Windows.Forms.Button();
            this.InfoLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LogFileTextBox
            // 
            this.LogFileTextBox.Enabled = false;
            this.LogFileTextBox.Font = new System.Drawing.Font("Fira Code", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LogFileTextBox.Location = new System.Drawing.Point(14, 30);
            this.LogFileTextBox.Multiline = true;
            this.LogFileTextBox.Name = "LogFileTextBox";
            this.LogFileTextBox.Size = new System.Drawing.Size(776, 389);
            this.LogFileTextBox.TabIndex = 0;
            this.LogFileTextBox.TextChanged += new System.EventHandler(this.LogFileTextBox_TextChanged);
            // 
            // CopyButton
            // 
            this.CopyButton.Location = new System.Drawing.Point(12, 422);
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(75, 23);
            this.CopyButton.TabIndex = 1;
            this.CopyButton.Text = "Copy";
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // InfoLabel
            // 
            this.InfoLabel.AutoSize = true;
            this.InfoLabel.Font = new System.Drawing.Font("Fira Code", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.InfoLabel.Location = new System.Drawing.Point(14, 8);
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(594, 19);
            this.InfoLabel.TabIndex = 2;
            this.InfoLabel.Text = "This form shows you logs and lets you view what\'s been happening.";
            // 
            // LogsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(805, 450);
            this.Controls.Add(this.InfoLabel);
            this.Controls.Add(this.CopyButton);
            this.Controls.Add(this.LogFileTextBox);
            this.Name = "LogsForm";
            this.Text = "LogsForm";
            this.Load += new System.EventHandler(this.LogsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox LogFileTextBox;
        private Button CopyButton;
        private Label InfoLabel;
    }
}