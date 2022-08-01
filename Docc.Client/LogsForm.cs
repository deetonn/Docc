using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Docc.Client.Connection;

namespace Docc.Client
{
    public partial class LogsForm : Form
    {
        public LogsForm()
        {
            InitializeComponent();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(LogFileTextBox.Text);
        }

        private void LogsForm_Load(object sender, EventArgs e)
        {
            foreach (var log in Global.LoadLogFileText())
            {
                LogFileTextBox.AppendText(log + Environment.NewLine);
            }
        }

        private void LogFileTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
