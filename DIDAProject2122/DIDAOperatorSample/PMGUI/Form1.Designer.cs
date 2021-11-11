
namespace PMGUI {
    partial class FormPM {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonReadConfigFile = new System.Windows.Forms.Button();
            this.textBoxConfigFileName = new System.Windows.Forms.TextBox();
            this.textBoxListConfigs = new System.Windows.Forms.TextBox();
            this.ReadNextLineButton = new System.Windows.Forms.Button();
            this.buttonReadCommand = new System.Windows.Forms.Button();
            this.textBoxReadCommand = new System.Windows.Forms.TextBox();
            this.textBoxDebug = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonReadConfigFile
            // 
            this.buttonReadConfigFile.Location = new System.Drawing.Point(13, 45);
            this.buttonReadConfigFile.Name = "buttonReadConfigFile";
            this.buttonReadConfigFile.Size = new System.Drawing.Size(151, 27);
            this.buttonReadConfigFile.TabIndex = 0;
            this.buttonReadConfigFile.Text = "Read Config File";
            this.buttonReadConfigFile.UseVisualStyleBackColor = true;
            this.buttonReadConfigFile.Click += new System.EventHandler(this.buttonReadConfigFile_Click);
            // 
            // textBoxConfigFileName
            // 
            this.textBoxConfigFileName.Location = new System.Drawing.Point(171, 45);
            this.textBoxConfigFileName.Name = "textBoxConfigFileName";
            this.textBoxConfigFileName.PlaceholderText = "Script Name";
            this.textBoxConfigFileName.Size = new System.Drawing.Size(582, 27);
            this.textBoxConfigFileName.TabIndex = 1;
            // 
            // textBoxListConfigs
            // 
            this.textBoxListConfigs.Location = new System.Drawing.Point(13, 78);
            this.textBoxListConfigs.Multiline = true;
            this.textBoxListConfigs.Name = "textBoxListConfigs";
            this.textBoxListConfigs.PlaceholderText = "Commands Readed";
            this.textBoxListConfigs.ReadOnly = true;
            this.textBoxListConfigs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxListConfigs.Size = new System.Drawing.Size(582, 221);
            this.textBoxListConfigs.TabIndex = 2;
            // 
            // ReadNextLineButton
            // 
            this.ReadNextLineButton.Location = new System.Drawing.Point(602, 79);
            this.ReadNextLineButton.Name = "ReadNextLineButton";
            this.ReadNextLineButton.Size = new System.Drawing.Size(151, 28);
            this.ReadNextLineButton.TabIndex = 3;
            this.ReadNextLineButton.Text = "Read next line";
            this.ReadNextLineButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ReadNextLineButton.UseVisualStyleBackColor = true;
            this.ReadNextLineButton.Click += new System.EventHandler(this.ReadNextLineButton_Click);
            // 
            // buttonReadCommand
            // 
            this.buttonReadCommand.Location = new System.Drawing.Point(13, 319);
            this.buttonReadCommand.Name = "buttonReadCommand";
            this.buttonReadCommand.Size = new System.Drawing.Size(151, 27);
            this.buttonReadCommand.TabIndex = 4;
            this.buttonReadCommand.Text = "Read Command";
            this.buttonReadCommand.UseVisualStyleBackColor = true;
            this.buttonReadCommand.Click += new System.EventHandler(this.buttonReadCommand_Click);
            // 
            // textBoxReadCommand
            // 
            this.textBoxReadCommand.Location = new System.Drawing.Point(171, 319);
            this.textBoxReadCommand.Name = "textBoxReadCommand";
            this.textBoxReadCommand.PlaceholderText = "Your Command";
            this.textBoxReadCommand.Size = new System.Drawing.Size(424, 27);
            this.textBoxReadCommand.TabIndex = 5;
            // 
            // textBoxDebug
            // 
            this.textBoxDebug.Location = new System.Drawing.Point(12, 352);
            this.textBoxDebug.Multiline = true;
            this.textBoxDebug.Name = "textBoxDebug";
            this.textBoxDebug.PlaceholderText = "Debug";
            this.textBoxDebug.ReadOnly = true;
            this.textBoxDebug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDebug.Size = new System.Drawing.Size(582, 221);
            this.textBoxDebug.TabIndex = 6;
            // 
            // FormPM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.textBoxDebug);
            this.Controls.Add(this.textBoxReadCommand);
            this.Controls.Add(this.buttonReadCommand);
            this.Controls.Add(this.ReadNextLineButton);
            this.Controls.Add(this.textBoxListConfigs);
            this.Controls.Add(this.textBoxConfigFileName);
            this.Controls.Add(this.buttonReadConfigFile);
            this.Name = "FormPM";
            this.Text = "Puppet Master";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPM_Closing);
            this.Load += new System.EventHandler(this.FormPM_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonReadConfigFile;
        private System.Windows.Forms.TextBox textBoxConfigFileName;
        private System.Windows.Forms.TextBox textBoxListConfigs;
        private System.Windows.Forms.Button ReadNextLineButton;
        private System.Windows.Forms.Button buttonReadCommand;
        private System.Windows.Forms.TextBox textBoxReadCommand;
        private System.Windows.Forms.TextBox textBoxDebug;
    }
}

