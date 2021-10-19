
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
            this.textBoxConfigFileName.Size = new System.Drawing.Size(582, 27);
            this.textBoxConfigFileName.TabIndex = 1;
            // 
            // textBoxListConfigs
            // 
            this.textBoxListConfigs.Location = new System.Drawing.Point(13, 78);
            this.textBoxListConfigs.Multiline = true;
            this.textBoxListConfigs.Name = "textBoxListConfigs";
            this.textBoxListConfigs.ReadOnly = true;
            this.textBoxListConfigs.Size = new System.Drawing.Size(582, 221);
            this.textBoxListConfigs.TabIndex = 2;
            // 
            // FormPM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBoxListConfigs);
            this.Controls.Add(this.textBoxConfigFileName);
            this.Controls.Add(this.buttonReadConfigFile);
            this.Name = "FormPM";
            this.Text = "Puppet Master";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonReadConfigFile;
        private System.Windows.Forms.TextBox textBoxConfigFileName;
        private System.Windows.Forms.TextBox textBoxListConfigs;
    }
}

