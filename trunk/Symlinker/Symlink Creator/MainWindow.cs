﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Symlink_Creator.Properties;

namespace Symlink_Creator
{
    ///<summary>
    /// This class manages the window
    ///</summary>
    public partial class MainWindow : Form
    {
        private readonly ToolTip _tip = new ToolTip();
        private bool _folder;


        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _tip.IsBalloon = true;
            comboBox1.SelectedIndex = 0;
            TypeSelector.SelectedIndex = 0;
        }

        #region Events

        // Manages the action of the "info" image
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            MessageBox.Show(
                "© 2010 Alejandro Mora Díaz \n Version: "+version+" \n e-mail: amora@plexip.com \n Thanks to Microsoft for the use of their shortcut arrow :)",
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Manages the link explore button
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();
            textBox1.Text = folderBrowser.SelectedPath;
        }

        // Manages the explore button
        private void button2_Click(object sender, EventArgs e)
        {
            // if _folder is true the folder browser will be shown
            if (_folder)
            {
                folderBrowser.ShowDialog();
                textBox3.Text = folderBrowser.SelectedPath;
            }
            else
            {
                filesBrowser.ShowDialog();
                textBox3.Text = filesBrowser.FileName;
            }
        }

        // Manages the create link button
        private void button3_Click(object sender, EventArgs e)
        {
            CreateLink();
        }


        // manages the type selector combobox
        private void TypeSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            Switcher();
        }


        // Manages de type selector mouse hover event
        private void TypeSelector_MouseHover(object sender, EventArgs e)
        {
            _tip.ToolTipIcon = ToolTipIcon.Info;
            _tip.UseAnimation = true;
            _tip.UseFading = true;
            _tip.AutoPopDelay = 10000;
            _tip.ToolTipTitle = "Symbolic Link type selector";
            _tip.SetToolTip(TypeSelector,
                            "With this option you can choose between creating file symbolic links; \nthis is using a file to point to another file, or folder symbolic links; this \nis using folders that point to other folders");
        }

        private void ComboBox1MouseHover(object sender, EventArgs e)
        {
            _tip.ToolTipIcon = ToolTipIcon.Info;
            _tip.UseAnimation = true;
            _tip.UseFading = true;
            _tip.AutoPopDelay = 5000;
            _tip.ToolTipTitle = "Symbolic Link types";
            _tip.SetToolTip(comboBox1,
                            "This option allows you to select the style of your symbolic link, either\nyou choose to use symbolic links, hard links or directory junctions.\n use symbolic links as a default");
        }

        #endregion

        /// <summary>
        /// Creates the link if the conditions are met
        /// </summary>
        private void CreateLink()
        {
            try
            {
                if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "")
                    // Everything need to be filled...
                    if (_folder && Directory.Exists(textBox1.Text) && Directory.Exists(textBox3.Text))
                        // Ask if the folders exist
                    {
                        string link = "\"" + textBox1.Text + "\\" + textBox2.Text + "\" ";
                            // concatenates the link name with the folder name and then it adds a pair of ", to allow using directories with spaces..

                        string[] directories = Directory.GetDirectories(textBox1.Text);
                            // gets the folders in the selected directory
                        if (directories.Any(e => e.Split('\\').Last().Equals(textBox2.Text)))
                            // looks for folders with the same name of the link name
                        {
                            // if found the program ask the user if he wants to delete the folder that is already there
                            DialogResult answer = MessageBox.Show(Resources.DialogFolderExists,
                                                                  Resources.DialogFolderExistsDialog,
                                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (answer == DialogResult.Yes)
                            {
                                // if the answer is yes, the folder is deleted in order to create a new one
                                string dir2Delete = directories.First(e => e.Split('\\').Last().Equals(textBox2.Text));
                                Directory.Delete(dir2Delete);
                                SendCommand(link);
                                return;
                            }
                            MessageBox.Show(Resources.LinkCreationAborted, Resources.LinkCreationAbortedWarning,
                                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                        else
                        {
                            SendCommand(link);
                        }
                    }
                    else if (Directory.Exists(textBox1.Text) && File.Exists(textBox3.Text))
                    {
                        // same thing as above... it just deletes files instead of folders
                        string link = "\"" + textBox1.Text + "\\" + textBox2.Text + "\" ";

                        string[] files = Directory.GetFiles(textBox1.Text);
                        if (files.Any(e => e.Split('\\').Last().Equals(textBox2.Text)))
                        {
                            DialogResult answer = MessageBox.Show(Resources.DialogDeleteFile,
                                                                  Resources.DialogDeleteFileWarning,
                                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (answer == DialogResult.Yes)
                            {
                                string file2Delete = files.First(e => e.Split('\\').Last().Equals(textBox2.Text));
                                File.Delete(file2Delete);
                                SendCommand(link);
                                return;
                            }
                            MessageBox.Show(Resources.LinkCreationAborted, Resources.LinkCreationAbortedWarning,
                                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                        else
                        {
                            SendCommand(link);
                        }
                    }
                    else
                        MessageBox.Show(Resources.FilesOrFolderNotExists, "Error", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                else
                    MessageBox.Show(Resources.FillBlanks, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// This method allows to switch modes between file or folder symlinks
        /// </summary>
        private void Switcher()
        {
            if (TypeSelector.SelectedIndex == 0)
            {
                groupBox1.Text = "Link Folder";
                groupBox2.Text = "Destination Folder";
                label2.Text = "Now give a name to the link:";
                label3.Text = "Please select the path to the real folder you want to link:";
                _folder = true;
            }
            else
            {
                groupBox1.Text = "Link File";
                groupBox2.Text = "Destination File";
                label2.Text = "Now give a name to your file:";
                label3.Text = "Please select the path to the real file you want to link:";
                _folder = false;
            }
        }

        /// <summary>
        /// This method build a string using the paramethers provided by the user, after that, it start a new
        /// cmd.exe process with the string just built.
        /// </summary>
        /// <param name="link">Path to the place you want your symlink</param>
        private void SendCommand(string link)
        {
            try
            {
                string target = "\"" + textBox3.Text + "\"";
                    // concatenates a pair of "", this is to make folders with spaces to work
                string typeLink = ComboBoxSelection();
                string directory = _folder ? "/D " : "";
                string stringCommand = "/c mklink " + directory + typeLink + link + target;
                var processStartInfo = new ProcessStartInfo
                                           {
                                               FileName = "cmd",
                                               Arguments = stringCommand,
                                               CreateNoWindow = true
                                           };
                Process.Start(processStartInfo);
                MessageBox.Show(Resources.LinkSuccessfullyCreated, "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show(Resources.CmdNotFound, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// This Method lets you select the type of link you want to create
        /// </summary>
        /// <returns>String with the type of link to create</returns>
        private string ComboBoxSelection()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 1:
                    return "/H ";
                case 2:
                    return "/J ";
                default:
                    return "";
            }
        }
    }
}