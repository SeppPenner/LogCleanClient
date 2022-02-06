// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogCleanClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    using Languages.Implementation;
    using Languages.Interfaces;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// The background clean process.
        /// </summary>
        private readonly BackgroundWorker backgroundClean = new();

        /// <summary>
        /// The language manager.
        /// </summary>
        private readonly ILanguageManager languageManager = new LanguageManager();

        /// <summary>
        /// The configuration.
        /// </summary>
        private Config config = new();

        /// <summary>
        /// The list of deleted files.
        /// </summary>
        private List<string> filesDeleted = new();

        /// <summary>
        /// The language.
        /// </summary>
        private ILanguage? language;

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            this.InitializeComponent();
            this.InitializeCaption();
            this.InitializeLanguageManager();
            this.LoadLanguagesToCombo();
            this.LoadConfig();
        }

        /// <summary>
        /// Imports the configuration.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The new <see cref="Config"/> object.</returns>
        private static Config ImportConfiguration(string fileName)
        {
            var xDocument = XDocument.Load(fileName);
            return CreateObjectFromString<Config>(xDocument) ?? new();
        }

        /// <summary>
        /// Creates the object from a <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="xDocument">The X document.</param>
        /// <returns>A new object of type <see cref="T"/>.</returns>
        private static T? CreateObjectFromString<T>(XDocument xDocument)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T?)xmlSerializer.Deserialize(new StringReader(xDocument.ToString()));
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                var location = Assembly.GetExecutingAssembly().Location;
                this.config = ImportConfiguration(Path.Combine(Directory.GetParent(location)?.FullName ?? string.Empty, "Config.xml"));
                this.InitBackgroundWorker();
            }
            catch (Exception ex)
            {
                if (this.language is null)
                {
                    MessageBox.Show(ex.Message, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                var error = this.language.GetWord("Error");
                MessageBox.Show(ex.Message, error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the button click to clear the logs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void ButtonClearLogsClick(object sender, EventArgs e)
        {
            this.button_ClearLogs.Enabled = false;
            this.filesDeleted = new List<string>();
            this.ProgressBarCleanProgress.Value = 0;
            this.backgroundClean.RunWorkerAsync();
        }

        /// <summary>
        /// Initializes the background worker.
        /// </summary>
        private void InitBackgroundWorker()
        {
            this.backgroundClean.WorkerReportsProgress = true;
            this.backgroundClean.WorkerSupportsCancellation = true;
            this.backgroundClean.DoWork += this.BackgroundCleanWork;
            this.backgroundClean.RunWorkerCompleted += this.BackgroundCleanCompleted;
            this.backgroundClean.ProgressChanged += this.BackgroundCleanReportProgress;
        }

        /// <summary>
        /// Handles the background clean work.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void BackgroundCleanWork(object sender, DoWorkEventArgs e)
        {
            double totalAmount = 0;

            // Get infos from Directory (how many files are in there?)
            for (var i = this.config.LogModels.Count - 1; i >= 0; i--)
            {
                if (Directory.Exists(this.config.LogModels[i].LogFolder))
                {
                    var d = new DirectoryInfo(this.config.LogModels[i].LogFolder);
                    this.config.LogModels[i].FileAmount = d.GetFiles().Length;
                    totalAmount += this.config.LogModels[i].FileAmount;
                }
                else
                {
                    this.config.LogModels.Remove(this.config.LogModels[i]);
                }
            }

            // Clean log folders
            double fileCount = 0;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var logModel in this.config.LogModels)
            {
                var filterOptions = logModel.FileFilter.Split('|');

                if (!Directory.Exists(logModel.LogFolder))
                {
                    continue;
                }

                var d = new DirectoryInfo(logModel.LogFolder);

                foreach (var file in d.GetFiles())
                {
                    if (filterOptions.Any(t => file.FullName.EndsWith(t)))
                    {
                        File.Delete(file.FullName);
                        this.filesDeleted.Add(file.FullName);
                        fileCount++;
                        this.backgroundClean.ReportProgress(Convert.ToInt32(fileCount / totalAmount * 100));
                    }
                }
            }
        }

        /// <summary>
        /// Handles the background clean progress event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void BackgroundCleanReportProgress(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressBarCleanProgress.Value = e.ProgressPercentage >= 100 ? 100 : e.ProgressPercentage;
        }

        /// <summary>
        /// Handles the background clean completed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void BackgroundCleanCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.language is null)
            {
                return;
            }

            this.button_ClearLogs.Enabled = true;
            var reportDialog = new ReportDialog();
            var searchedDirectories = this.language.GetWord("SearchedDirectories");
            reportDialog.AddTextToRichTextBox(searchedDirectories + Environment.NewLine);
            var amount = this.language.GetWord("Amount");
            reportDialog.AddTextToRichTextBox(amount + this.config.LogModels.Count + Environment.NewLine);

            foreach (var logModel in this.config.LogModels)
            {
                var withFilter = this.language.GetWord("WithFilter");
                reportDialog.AddTextToRichTextBox(logModel.LogFolder + withFilter + logModel.FileFilter +
                                                  Environment.NewLine);
            }

            reportDialog.AddTextToRichTextBox(
                "----------------------------------------------------------------------------" +
                "---------------------------------------------------------------------------------------------------" +
                "-------------------------------------------" + Environment.NewLine);
            var deletedFiles = this.language.GetWord("DeletedFiles");
            reportDialog.AddTextToRichTextBox(deletedFiles + Environment.NewLine);
            amount = this.language.GetWord("Amount");
            reportDialog.AddTextToRichTextBox(amount + this.filesDeleted.Count + Environment.NewLine);

            foreach (var file in this.filesDeleted)
            {
                reportDialog.AddTextToRichTextBox(file + Environment.NewLine);
            }

            reportDialog.AddTextToRichTextBox(
                "----------------------------------------------------------------------------" +
                "---------------------------------------------------------------------------------------------------" +
                "-------------------------------------------" + Environment.NewLine);
            reportDialog.ShowDialog();
        }

        /// <summary>
        /// Initializes the language manager.
        /// </summary>
        private void InitializeLanguageManager()
        {
            this.languageManager.SetCurrentLanguage("de-DE");
            this.languageManager.OnLanguageChanged += this.OnLanguageChanged;
            this.language = this.languageManager.GetCurrentLanguage();
        }

        /// <summary>
        /// Loads the languages to the combo box.
        /// </summary>
        private void LoadLanguagesToCombo()
        {
            foreach (var localLanguage in this.languageManager.GetLanguages())
            {
                this.comboBoxLanguage.Items.Add(localLanguage.Name);
            }

            this.comboBoxLanguage.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the combo box selected event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void ComboBoxLanguageSelectedIndexChanged(object sender, EventArgs e)
        {
            this.languageManager.SetCurrentLanguageFromName(this.comboBoxLanguage.SelectedItem.ToString());
        }

        /// <summary>
        /// Initializes the caption.
        /// </summary>
        private void InitializeCaption()
        {
            this.Text = Application.ProductName + @" " + Application.ProductVersion;
        }

        /// <summary>
        /// Handles the language changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            this.button_ClearLogs.Text = this.languageManager.GetCurrentLanguage().GetWord("ClearLogs");
            this.language = this.languageManager.GetCurrentLanguage();
        }
    }
}