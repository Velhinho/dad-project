using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibraryPM;

namespace PMGUI {
    public partial class FormPM : Form {

        private PM_logic PM = new PM_logic();

        public FormPM() {
            InitializeComponent();
        }

        private void buttonReadConfigFile_Click(object sender, EventArgs e) {
            buttonReadConfigFile.Enabled = false;
            textBoxConfigFileName.Enabled = false;
            ReadNextLineButton.Enabled = false;
            PM.readConfigFile(textBoxConfigFileName.Text);

            textBoxListConfigs.Text = PM.listarConfig();
        }

        private void ReadNextLineButton_Click(object sender, EventArgs e) {
            textBoxConfigFileName.Enabled = false;

            bool end_of_file = PM.readNextLine(textBoxConfigFileName.Text);

            if (end_of_file) {
                ReadNextLineButton.Enabled = false;
                buttonReadConfigFile.Enabled = false;
            }

            textBoxListConfigs.Text = PM.listarConfig();
        }
    }
}
