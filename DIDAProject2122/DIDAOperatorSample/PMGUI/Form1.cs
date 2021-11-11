using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PMGUI {
    public partial class FormPM : Form {

        private PM_logic PM;

        public FormPM() {
            InitializeComponent();
            PM = new PM_logic(this);
        }

        private void buttonReadConfigFile_Click(object sender, EventArgs e) {
            buttonReadConfigFile.Enabled = false;
            textBoxConfigFileName.Enabled = false;
            ReadNextLineButton.Enabled = false;
            PM.readConfigFile(textBoxConfigFileName.Text);

            textBoxListConfigs.Text = PM.listarComandos();
        }

        private void ReadNextLineButton_Click(object sender, EventArgs e) {
            textBoxConfigFileName.Enabled = false;

            bool end_of_file = PM.readNextLine(textBoxConfigFileName.Text);

            if (end_of_file) {
                ReadNextLineButton.Enabled = false;
                buttonReadConfigFile.Enabled = false;
            }

            textBoxListConfigs.Text = PM.listarComandos();
        }

        private void buttonReadCommand_Click(object sender, EventArgs e) {
            PM.HandleNextLine(textBoxReadCommand.Text);

            textBoxReadCommand.Text = "";
            textBoxListConfigs.Text = PM.listarComandos();
        }

        public void AddDebug(string m) { 
            textBoxDebug.Text += m + "\r\n"; 
        }

        private void FormPM_Load(object sender, EventArgs e) {

        }

        private void FormPM_Closing(object sender, FormClosingEventArgs e) {
            PM.ServerShutdown();
        }

    }
}
