using System;
using System.Windows.Forms;

namespace ViberToPDF_Converter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            var goDo = new Implementation();

            var ofd = new OpenFileDialog
            {
                InitialDirectory = "@c:\\",
                Filter = @"csv Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };


            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    goDo.FilePath = ofd.FileName;
                    goDo.ReadFile();
                    goDo.PrintData();
                }
                catch (Exception ex)
                {
                    
                    MessageBox.Show(@"Could not read the file, please check the file and try again. " + ex.Message);
                }

            }
        }
    }
}
