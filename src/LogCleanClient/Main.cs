// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogCleanClient;

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
        this.InitBackgroundWorker();
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
    /// Gets the file filter options of a <see cref="LogModel"/>.
    /// </summary>
    /// <param name="logModel">The log model.</param>
    /// <returns>The filter options without the empty entries.</returns>
    private static string[] GetFilterOptions(LogModel logModel)
    {
        return logModel.FileFilter.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// Checks whether a file matches one of the filter options.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="filterOptions">The filter options.</param>
    /// <returns>A value indicating whether the file matches one of the filter options or not.</returns>
    private static bool MatchesFilter(FileInfo file, string[] filterOptions)
    {
        return filterOptions.Any(filterOption => file.FullName.EndsWith(filterOption));
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
        }
        catch (Exception ex)
        {
            this.ShowError(ex);
        }
    }

    /// <summary>
    /// Shows an exception in a message box.
    /// </summary>
    /// <param name="ex">The exception.</param>
    private void ShowError(Exception ex)
    {
        var title = this.language?.GetWord("Error") ?? Application.ProductName;
        var text = $"{ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.StackTrace}";
        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        this.backgroundClean.DoWork += this.BackgroundCleanWork!;
        this.backgroundClean.RunWorkerCompleted += this.BackgroundCleanCompleted!;
        this.backgroundClean.ProgressChanged += this.BackgroundCleanReportProgress!;
    }

    /// <summary>
    /// Handles the background clean work.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void BackgroundCleanWork(object sender, DoWorkEventArgs e)
    {
        double totalAmount = 0;

        // Get infos from Directory (how many files in there match the filter?)
        for (var i = this.config.LogModels.Count - 1; i >= 0; i--)
        {
            var logModel = this.config.LogModels[i];

            if (!Directory.Exists(logModel.LogFolder))
            {
                this.config.LogModels.RemoveAt(i);
                continue;
            }

            var filterOptions = GetFilterOptions(logModel);

            if (filterOptions.Length == 0)
            {
                logModel.FileAmount = 0;
                continue;
            }

            var d = new DirectoryInfo(logModel.LogFolder);
            logModel.FileAmount = d.GetFiles().Count(file => MatchesFilter(file, filterOptions));
            totalAmount += logModel.FileAmount;
        }

        // Clean log folders
        double fileCount = 0;

        foreach (var logModel in this.config.LogModels)
        {
            var filterOptions = GetFilterOptions(logModel);

            // A log model without a usable filter entry is skipped, an empty entry would match every file.
            if (filterOptions.Length == 0 || !Directory.Exists(logModel.LogFolder))
            {
                continue;
            }

            var d = new DirectoryInfo(logModel.LogFolder);

            foreach (var file in d.GetFiles())
            {
                if (!MatchesFilter(file, filterOptions))
                {
                    continue;
                }

                File.Delete(file.FullName);
                this.filesDeleted.Add(file.FullName);
                fileCount++;

                if (totalAmount > 0)
                {
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
        this.button_ClearLogs.Enabled = true;

        // The background worker swallows every exception of the clean run into the error property.
        if (e.Error is not null)
        {
            this.ShowError(e.Error);
        }

        if (this.language is null)
        {
            return;
        }

        using var reportDialog = new ReportDialog();
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
        this.languageManager.OnLanguageChanged += this.OnLanguageChanged!;
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
        var selectedItem = this.comboBoxLanguage.SelectedItem?.ToString();

        if (string.IsNullOrWhiteSpace(selectedItem))
        {
            return;
        }

        this.languageManager.SetCurrentLanguageFromName(selectedItem);
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
