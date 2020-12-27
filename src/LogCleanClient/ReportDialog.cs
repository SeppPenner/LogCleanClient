// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportDialog.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The report dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogCleanClient
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The report dialog.
    /// </summary>
    public partial class ReportDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportDialog"/> class.
        /// </summary>
        public ReportDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Adds text to the rich text box.
        /// </summary>
        /// <param name="text">The text.</param>
        public void AddTextToRichTextBox(string text)
        {
            this.RichTextBoxReport.AppendText(text);
        }

        /// <summary>
        /// Handles the ok click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OkClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}