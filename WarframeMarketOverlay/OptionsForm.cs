using System;
using System.Linq;
using System.Windows.Forms;
using WarframeMarketOverlay.Properties;
using Hotkeys;
using HtmlAgilityPack;
using System.IO;
using Microsoft.Win32;

namespace WarframeMarketOverlay
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {//Initializes the OptionsForm and sets the values of its components according to settings

            InitializeComponent();
            this.Text = "Options";
            comboBoxKey.SelectedIndex = Properties.Settings.Default.Key_Index;
            int temp = Settings.Default.Modifier_Value;
            checkBoxAlt.Checked = new int[] { 1, 3, 5, 7, 9, 11, 13, 15 }.Contains(temp);
            checkBoxCtrl.Checked = new int[] { 2, 3, 6, 7, 10, 11, 14, 15 }.Contains(temp);
            checkBoxShift.Checked = new int[] { 4, 5, 6, 7, 12, 13, 14, 15 }.Contains(temp);
            checkBoxWin.Checked = new int[] { 8, 9, 10, 11, 12, 13, 14, 15 }.Contains(temp);

            //Check startup settings
            new System.Security.Permissions.RegistryPermission(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (registryKey.GetValue(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)) != null)
                {
                    checkBoxStartup.Checked = true;
                }
                registryKey.Close();
            }
            finally
            {
                System.Security.Permissions.RegistryPermission.RevertAssert();
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.ApplyChanges();
                this.Close();
            }
            catch (Exception a)
            {
                MessageBox.Show(this, a.Message, "Apply Changes Error");
            }
        }

        private int GenerateModifierValue()
        {//Generates modifier int according to checked components

            int result = Constants.NOMOD;
            if (checkBoxAlt.Checked)
                result += Constants.ALT;
            if (checkBoxCtrl.Checked)
                result += Constants.CTRL;
            if (checkBoxShift.Checked)
                result += Constants.SHIFT;
            if (checkBoxWin.Checked)
                result += Constants.WIN;
            return result;
        }

        private void ApplyChanges()
        {//Applies changes to settings

            Settings.Default["Modifier_Value"] = GenerateModifierValue();
            Enum.TryParse(comboBoxKey.Text, out Keys key);
            Settings.Default.Key_Value = key;
            Settings.Default.Key_Index = comboBoxKey.SelectedIndex;
            Settings.Default.Save();

            new System.Security.Permissions.RegistryPermission(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (checkBoxStartup.Checked)
                {
                    if (registryKey.GetValue(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)) == null)
                        registryKey.SetValue(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location), Application.ExecutablePath);
                }
                else
                {
                    if (registryKey.GetValue(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)) != null)
                        registryKey.DeleteValue(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));
                }
                registryKey.Close();
            }
            finally
            {
                System.Security.Permissions.RegistryPermission.RevertAssert();
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                labelUpdateDone.Text = "";
                backgroundWorkerUpdate.RunWorkerAsync();
            }
            catch (Exception a)
            {
                MessageBox.Show(this, a.Message, "Update error");
            }
        }

        private void backgroundWorkerUpdate_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            var document = htmlWeb.Load("http://warframe.wikia.com/wiki/Void_Relic/ByRewards/SimpleTable");
            var contentNode = document.GetElementbyId("mw-content-text");
            var items = contentNode.FirstChild.ChildNodes;

            //Create file to write to it
            using (FileStream fileStream = new FileStream("Items.bin", FileMode.Create))
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    string currentItem, lastItem, checkHolder2;
                    //Add first item
                    int i = 3;
                    currentItem = items[i].ChildNodes[1].InnerText + ' ' + items[i].ChildNodes[2].InnerText;
                    binaryWriter.Write(currentItem + '\n'); //change to \0
                    lastItem = currentItem;

                    checkHolder2 = items[i].ChildNodes[2].InnerText;
                    if (checkHolder2.Contains("Chassis"))
                        checkHolder2 = "chassis";
                    else if (checkHolder2.Contains("Neuroptics"))
                        checkHolder2 = "neuroptics";
                    else if (checkHolder2.Contains("Systems"))
                        checkHolder2 = "systems";
                    binaryWriter.Write((items[i].ChildNodes[1].InnerText.Replace("&amp;", "and") + '_' + checkHolder2 + '\n').ToLower().Replace(' ', '_'));    //change to \0 if needed

                    //Add the rest of the items
                    for (i = 5; i < items.Count; i += 2)
                    {
                        currentItem = items[i].ChildNodes[1].InnerText + ' ' + items[i].ChildNodes[2].InnerText;
                        if (currentItem != lastItem)
                        {
                            binaryWriter.Write(currentItem + '\n'); //change to \0
                            lastItem = currentItem;

                            checkHolder2 = items[i].ChildNodes[2].InnerText;
                            if (checkHolder2.Contains("Chassis"))
                                checkHolder2 = "chassis";
                            else if (checkHolder2.Contains("Neuroptics"))
                                checkHolder2 = "neuroptics";
                            else if (checkHolder2.Contains("Systems"))
                                checkHolder2 = "systems";
                            binaryWriter.Write((items[i].ChildNodes[1].InnerText.Replace("&amp;", "and") + '_' + checkHolder2 + '\n').ToLower().Replace(' ', '_'));    //change to \o if needed
                        }
                    }
                }
        }

        private void backgroundWorkerUpdate_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)  //Test text
        {
            labelUpdateDone.Text = "Done";
            backgroundWorkerUpdate.Dispose();
            //try
            //{
            //    using (FileStream readFile = new FileStream("Items.bin", FileMode.Open))
            //    using (BinaryReader readBin = new BinaryReader(readFile))
            //    using (StreamWriter writeFile = new StreamWriter("Items.txt"))
            //        while (readBin.BaseStream.Position != readBin.BaseStream.Length)
            //        {
            //            writeFile.Write(readBin.ReadString().Replace("&amp;", "&"));
            //        }
            //}
            //catch (Exception a)
            //{
            //    MessageBox.Show(a.Message);
            //}
        }
    }
}
