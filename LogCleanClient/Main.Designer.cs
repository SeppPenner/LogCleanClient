namespace LogCleanClient
{
    partial class Main
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.button_ClearLogs = new System.Windows.Forms.Button();
            this.progressBar_CleanProgress = new System.Windows.Forms.ProgressBar();
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button_ClearLogs
            // 
            this.button_ClearLogs.Location = new System.Drawing.Point(12, 68);
            this.button_ClearLogs.Name = "button_ClearLogs";
            this.button_ClearLogs.Size = new System.Drawing.Size(272, 23);
            this.button_ClearLogs.TabIndex = 0;
            this.button_ClearLogs.Text = "Logs löschen";
            this.button_ClearLogs.UseVisualStyleBackColor = true;
            this.button_ClearLogs.Click += new System.EventHandler(this.button_ClearLogs_Click);
            // 
            // progressBar_CleanProgress
            // 
            this.progressBar_CleanProgress.Location = new System.Drawing.Point(12, 39);
            this.progressBar_CleanProgress.Name = "progressBar_CleanProgress";
            this.progressBar_CleanProgress.Size = new System.Drawing.Size(272, 23);
            this.progressBar_CleanProgress.TabIndex = 1;
            // 
            // comboBoxLanguage
            // 
            this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLanguage.FormattingEnabled = true;
            this.comboBoxLanguage.Location = new System.Drawing.Point(12, 12);
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.Size = new System.Drawing.Size(272, 21);
            this.comboBoxLanguage.TabIndex = 2;
            this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxLanguage_SelectedIndexChanged);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 104);
            this.Controls.Add(this.comboBoxLanguage);
            this.Controls.Add(this.progressBar_CleanProgress);
            this.Controls.Add(this.button_ClearLogs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Log Clear Client";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_ClearLogs;
        private System.Windows.Forms.ProgressBar progressBar_CleanProgress;
        private System.Windows.Forms.ComboBox comboBoxLanguage;
    }
}

