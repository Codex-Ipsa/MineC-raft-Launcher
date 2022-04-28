﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCLauncher
{
    public partial class Update : Form
    {
        public Update()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            string currentPath = Directory.GetCurrentDirectory();

            if(Globals.isDev == true)
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://dejvoss.cz/launcher-data/MCLauncherDevUpdater.exe", currentPath + "\\MCLauncherDevUpdater.exe");
                }

                System.Diagnostics.Process.Start("CMD.exe", $"/C MCLauncherDevUpdater.exe");
                Application.Exit();
            }
            else
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://dejvoss.cz/launcher-data/MCLauncherUpdater.exe", currentPath + "\\MCLauncherUpdater.exe");
                }

                System.Diagnostics.Process.Start("CMD.exe", $"/C MCLauncherUpdater.exe");
                Application.Exit();
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
