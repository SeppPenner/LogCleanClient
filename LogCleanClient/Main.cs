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

namespace LogCleanClient
{
    public partial class Main : Form
    {
        private readonly BackgroundWorker _backgroundClean = new BackgroundWorker();

        private readonly ILanguageManager _lm = new LanguageManager();


        private Config _config = new Config();
        private List<string> _filesDeleted = new List<string>();
        private Language _lang;

        public Main()
        {
            InitializeComponent();
            InitializeCaption();
            InitializeLanguageManager();
            LoadLanguagesToCombo();
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                var location = Assembly.GetExecutingAssembly().Location;
                if (location != null)
                    _config = ImportConfiguration(Path.Combine(Directory.GetParent(location).FullName, "Config.xml"));
                InitBackgroundWorker();
            }
            catch (Exception ex)
            {
                var error = _lang.GetWord("Error");
                MessageBox.Show(ex.Message, error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button_ClearLogs_Click(object sender, EventArgs e)
        {
            button_ClearLogs.Enabled = false;
            _filesDeleted = new List<string>();
            progressBar_CleanProgress.Value = 0;
            _backgroundClean.RunWorkerAsync();
        }


        private void InitBackgroundWorker()
        {
            _backgroundClean.WorkerReportsProgress = true;
            _backgroundClean.WorkerSupportsCancellation = true;
            _backgroundClean.DoWork += BackgroundClean_Work;
            _backgroundClean.RunWorkerCompleted += BackgroundClean_Completed;
            _backgroundClean.ProgressChanged += BackgroundClean_ReportProgress;
        }

        private Config ImportConfiguration(string fileName)
        {
            var xDocument = XDocument.Load(fileName);
            return CreateObjectFromString<Config>(xDocument);
        }

        private T CreateObjectFromString<T>(XDocument xDocument)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T) xmlSerializer.Deserialize(new StringReader(xDocument.ToString()));
        }


        private void BackgroundClean_Work(object sender, DoWorkEventArgs e)
        {
            double totalAmount = 0;
            //Get infos from Directory (how many files are in there?)

            for (var i = _config.LogModels.Count - 1; i >= 0; i--)
                if (Directory.Exists(_config.LogModels[i].LogFolder))
                {
                    var d = new DirectoryInfo(_config.LogModels[i].LogFolder);
                    _config.LogModels[i].FileAmount = d.GetFiles().Length;
                    totalAmount += _config.LogModels[i].FileAmount;
                }
                else
                {
                    _config.LogModels.Remove(_config.LogModels[i]);
                }

            //Clean log folders
            double fileCount = 0;
            foreach (var lModel in _config.LogModels)
            {
                var filterOptions = lModel.FileFilter.Split('|');
                if (!Directory.Exists(lModel.LogFolder)) continue;
                var d = new DirectoryInfo(lModel.LogFolder);
                foreach (var file in d.GetFiles())
                    if (filterOptions.Any(t => file.FullName.EndsWith(t)))
                    {
                        File.Delete(file.FullName);
                        _filesDeleted.Add(file.FullName);
                        fileCount++;
                        _backgroundClean.ReportProgress(Convert.ToInt32(fileCount / totalAmount * 100));
                    }
            }
        }

        private void BackgroundClean_ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            progressBar_CleanProgress.Value = e.ProgressPercentage >= 100 ? 100 : e.ProgressPercentage;
        }

        private void BackgroundClean_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            button_ClearLogs.Enabled = true;
            var reportDialog = new ReportDialog();
            var searchedDirectories = _lang.GetWord("SearchedDirectories");
            reportDialog.AddTextToRichtextBox(searchedDirectories + Environment.NewLine);
            var amount = _lang.GetWord("Amount");
            reportDialog.AddTextToRichtextBox(amount + _config.LogModels.Count + Environment.NewLine);
            foreach (var lModel in _config.LogModels)
            {
                var withFilter = _lang.GetWord("WithFilter");
                reportDialog.AddTextToRichtextBox(lModel.LogFolder + withFilter + lModel.FileFilter +
                                                  Environment.NewLine);
            }
            reportDialog.AddTextToRichtextBox(
                "----------------------------------------------------------------------------" +
                "---------------------------------------------------------------------------------------------------" +
                "-------------------------------------------" + Environment.NewLine);
            var deletedFiles = _lang.GetWord("DeletedFiles");
            reportDialog.AddTextToRichtextBox(deletedFiles + Environment.NewLine);
            amount = _lang.GetWord("Amount");
            reportDialog.AddTextToRichtextBox(amount + _filesDeleted.Count + Environment.NewLine);
            foreach (var file in _filesDeleted)
                reportDialog.AddTextToRichtextBox(file + Environment.NewLine);
            reportDialog.AddTextToRichtextBox(
                "----------------------------------------------------------------------------" +
                "---------------------------------------------------------------------------------------------------" +
                "-------------------------------------------" + Environment.NewLine);
            reportDialog.ShowDialog();
        }

        private void InitializeLanguageManager()
        {
            _lm.SetCurrentLanguage("de-DE");
            _lm.OnLanguageChanged += OnLanguageChanged;
        }

        private void LoadLanguagesToCombo()
        {
            foreach (var language in _lm.GetLanguages())
                comboBoxLanguage.Items.Add(language.Name);
            comboBoxLanguage.SelectedIndex = 0;
        }

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            _lm.SetCurrentLanguageFromName(comboBoxLanguage.SelectedItem.ToString());
        }

        private void InitializeCaption()
        {
            Text = Application.ProductName + @" " + Application.ProductVersion;
        }

        private void OnLanguageChanged(object sender, EventArgs eventArgs)
        {
            button_ClearLogs.Text = _lm.GetCurrentLanguage().GetWord("ClearLogs");
        }
    }
}