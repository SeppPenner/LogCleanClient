using System;
using System.Windows.Forms;

namespace LogCleanClient
{
    public partial class ReportDialog : Form
    {
        public ReportDialog()
        {
            InitializeComponent();
        }


        private void button_OK_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void AddTextToRichtextBox(string text)
        {
            richTextBox_Report.AppendText(text);
        }
    }
}