namespace Docc.Client
{
    partial class MainChatForm
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Welcome to Docc!"}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.InactiveCaption, null);
            this.SessionIdLabel = new System.Windows.Forms.Label();
            this.ChatBox = new System.Windows.Forms.ListView();
            this.InputTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.CustomNameEnabled = new System.Windows.Forms.CheckBox();
            this.CustomNameTextBox = new System.Windows.Forms.TextBox();
            this.CustomColorEnabled = new System.Windows.Forms.CheckBox();
            this.CustomColorTextBox = new System.Windows.Forms.TextBox();
            this.ErrorBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.ViewLogsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SessionIdLabel
            // 
            this.SessionIdLabel.AutoSize = true;
            this.SessionIdLabel.Font = new System.Drawing.Font("Fira Code", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SessionIdLabel.Location = new System.Drawing.Point(12, 9);
            this.SessionIdLabel.Name = "SessionIdLabel";
            this.SessionIdLabel.Size = new System.Drawing.Size(72, 19);
            this.SessionIdLabel.TabIndex = 0;
            this.SessionIdLabel.Text = "Session";
            this.SessionIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChatBox
            // 
            this.ChatBox.AllowDrop = true;
            this.ChatBox.AutoArrange = false;
            this.ChatBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ChatBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ChatBox.Cursor = System.Windows.Forms.Cursors.No;
            this.ChatBox.Font = new System.Drawing.Font("Fira Code", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ChatBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ChatBox.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.ChatBox.Location = new System.Drawing.Point(12, 30);
            this.ChatBox.Name = "ChatBox";
            this.ChatBox.Size = new System.Drawing.Size(639, 353);
            this.ChatBox.TabIndex = 1;
            this.ChatBox.UseCompatibleStateImageBehavior = false;
            this.ChatBox.View = System.Windows.Forms.View.List;
            // 
            // InputTextBox
            // 
            this.InputTextBox.Location = new System.Drawing.Point(12, 390);
            this.InputTextBox.Multiline = true;
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.Size = new System.Drawing.Size(574, 34);
            this.InputTextBox.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Fira Code", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(592, 390);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(59, 35);
            this.button1.TabIndex = 3;
            this.button1.Text = ">=";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CustomNameEnabled
            // 
            this.CustomNameEnabled.AutoSize = true;
            this.CustomNameEnabled.Location = new System.Drawing.Point(658, 123);
            this.CustomNameEnabled.Name = "CustomNameEnabled";
            this.CustomNameEnabled.Size = new System.Drawing.Size(121, 19);
            this.CustomNameEnabled.TabIndex = 4;
            this.CustomNameEnabled.Text = "Use custom name";
            this.CustomNameEnabled.UseVisualStyleBackColor = true;
            // 
            // CustomNameTextBox
            // 
            this.CustomNameTextBox.Font = new System.Drawing.Font("Fira Code", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CustomNameTextBox.Location = new System.Drawing.Point(658, 148);
            this.CustomNameTextBox.MaxLength = 16;
            this.CustomNameTextBox.Name = "CustomNameTextBox";
            this.CustomNameTextBox.Size = new System.Drawing.Size(130, 23);
            this.CustomNameTextBox.TabIndex = 5;
            // 
            // CustomColorEnabled
            // 
            this.CustomColorEnabled.AutoSize = true;
            this.CustomColorEnabled.Location = new System.Drawing.Point(657, 177);
            this.CustomColorEnabled.Name = "CustomColorEnabled";
            this.CustomColorEnabled.Size = new System.Drawing.Size(100, 19);
            this.CustomColorEnabled.TabIndex = 6;
            this.CustomColorEnabled.Text = "Custom Color";
            this.CustomColorEnabled.UseVisualStyleBackColor = true;
            // 
            // CustomColorTextBox
            // 
            this.CustomColorTextBox.Location = new System.Drawing.Point(658, 202);
            this.CustomColorTextBox.MaxLength = 16;
            this.CustomColorTextBox.Name = "CustomColorTextBox";
            this.CustomColorTextBox.Size = new System.Drawing.Size(130, 23);
            this.CustomColorTextBox.TabIndex = 7;
            // 
            // ErrorBox
            // 
            this.ErrorBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.ErrorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ErrorBox.Enabled = false;
            this.ErrorBox.Font = new System.Drawing.Font("Fira Code", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ErrorBox.Location = new System.Drawing.Point(657, 231);
            this.ErrorBox.Multiline = true;
            this.ErrorBox.Name = "ErrorBox";
            this.ErrorBox.ReadOnly = true;
            this.ErrorBox.Size = new System.Drawing.Size(131, 193);
            this.ErrorBox.TabIndex = 8;
            this.ErrorBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Gray;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(760, -6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(43, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "X";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Gray;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(728, -6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(39, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "⎯";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ViewLogsButton
            // 
            this.ViewLogsButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ViewLogsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ViewLogsButton.Font = new System.Drawing.Font("Fira Code", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ViewLogsButton.Location = new System.Drawing.Point(-1, 430);
            this.ViewLogsButton.Name = "ViewLogsButton";
            this.ViewLogsButton.Size = new System.Drawing.Size(111, 23);
            this.ViewLogsButton.TabIndex = 11;
            this.ViewLogsButton.Text = "View Logs";
            this.ViewLogsButton.UseVisualStyleBackColor = false;
            this.ViewLogsButton.Click += new System.EventHandler(this.ViewLogsButton_Click);
            // 
            // MainChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ViewLogsButton);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.ErrorBox);
            this.Controls.Add(this.CustomColorTextBox);
            this.Controls.Add(this.CustomColorEnabled);
            this.Controls.Add(this.CustomNameTextBox);
            this.Controls.Add(this.CustomNameEnabled);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.InputTextBox);
            this.Controls.Add(this.ChatBox);
            this.Controls.Add(this.SessionIdLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainChatForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainChatForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label SessionIdLabel;
        private ListView ChatBox;
        private TextBox InputTextBox;
        private Button button1;
        private CheckBox CustomNameEnabled;
        private TextBox CustomNameTextBox;
        private CheckBox CustomColorEnabled;
        private TextBox CustomColorTextBox;
        private TextBox ErrorBox;
        private Button button2;
        private Button button3;
        private Button ViewLogsButton;
    }
}