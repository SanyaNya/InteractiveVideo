using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InteractiveVideo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            videoPlayer.Size = new Size(Size.Width - 100, Size.Height - 30);

            loadButton.Location = new Point(loadButton.Location.X, Size.Height - 62);
            saveButton.Location = new Point(saveButton.Location.X, Size.Height - 107);
            saveTextBox.Location = new Point(saveTextBox.Location.X, Size.Height - 88);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveTextBox.Text == "")
            {
                MessageBox.Show("Введите название файла сохранения");
                return;
            }

            Scene.ReWrite("Saves/" + saveTextBox.Text + ".txt", Scene.currentScenePath);

            MessageBox.Show("Сохранено под названием " + saveTextBox.Text);
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Scene.Start(Scene.GetText(openFileDialog.FileName));
            }
        }
    }
}
