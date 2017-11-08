using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;

namespace MJRLegends_Backup_Tool
{
    public partial class Form1 : DevComponents.DotNetBar.Metro.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            XMLRead();
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            String path = fbd.SelectedPath;
            txtFolderName.Text = path;
            btnSelectFolder.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBoxTasks.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a entry first", "ERROR!");
            }
            else
            {
                listBoxTasks.Items.Remove(listBoxTasks.SelectedItem.ToString());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(txtName.Text != "" && txtFolderName.Text != "")
            {
                listBoxTasks.Items.Add(txtName.Text + " > " + txtFolderName.Text);
            }
            else
            {
                MessageBox.Show("Please make sure all fields are filled out before adding a entry!", "ERROR!");
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            txtStatus.AppendText("Creating Backups!..." + "\n");
            for (int i = 0; i < listBoxTasks.Items.Count; i++)
            {
                ZipFile zip = new ZipFile();
                zip.ParallelDeflateThreshold = -1;
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.Default;

                String listItem = listBoxTasks.Items[i].ToString();
                txtStatus.AppendText("Creating Backup for " + listItem.Substring(0, listItem.IndexOf(">") - 1) + "..." + "\n");
                zip.AddDirectory(listItem.Substring(listItem.IndexOf(" > ") + 3));

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = @"C:\";
                saveFileDialog1.Title = "Save Backup for " + listItem.Substring(0, listItem.IndexOf(">") - 1);
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.DefaultExt = ".zip";
                saveFileDialog1.Filter = "ZIP file (*.zip)|*.zip";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.FileName = listItem.Substring(0, listItem.IndexOf(">") - 1) + " " + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".zip";
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    zip.Save(saveFileDialog1.FileName);
                }

                zip.Dispose();

                txtStatus.AppendText("Backup for " + listItem.Substring(0, listItem.IndexOf(">") - 1) + " is now  Completed!..." + "\n");
            }
            txtStatus.AppendText("Backups Completed!..." + "\n");
        }

        private void savetoxml()
        {
            if (File.Exists(Application.StartupPath + "Data" + ".xml"))
                File.Delete(Application.StartupPath + "Data" + ".xml");

            using (XmlWriter writer = XmlWriter.Create(Application.StartupPath + "/Data" + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Entries");
                writer.WriteAttributeString("NumOfEntries", "" + listBoxTasks.Items.Count);
                for (int i = 0; i < listBoxTasks.Items.Count; i++)
                {
                    String listItem = listBoxTasks.Items[i].ToString();
                    writer.WriteAttributeString("NameEntry" + i, listItem.Substring(0, listItem.IndexOf(">") - 1));
                    writer.WriteAttributeString("FolderEntry" + i, listItem.Substring(listItem.IndexOf(" > ") + 3));
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
        }
        private void XMLRead()
        {
            if (File.Exists(Application.StartupPath + "/Data" + ".xml"))
            {
                if (listBoxTasks.Items.Count == 0)
                {
                    XmlTextReader reader = new XmlTextReader(Application.StartupPath + "/Data" + ".xml");
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "Entries")
                            {
                                for (int i = 0; i < Convert.ToInt32(reader.GetAttribute("NumOfEntries")); i++)
                                {
                                    listBoxTasks.Items.Add(reader.GetAttribute("NameEntry" + i) + " > " + reader.GetAttribute("FolderEntry" + i));
                                }
                            }
                        }
                    }
                    reader.Close();
                }
            }
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            if (listBoxTasks.Items.Count > 0)
            {
                savetoxml();
            }
            else
                MessageBox.Show("Please make sure have more or more entries before saving!", "ERROR!");
        }
    }
}
