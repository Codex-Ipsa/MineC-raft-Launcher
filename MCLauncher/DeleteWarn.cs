﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MCLauncher
{
    public partial class DeleteWarn : Form
    {
        public string name = "";

        public DeleteWarn(string nameI)
        {
            InitializeComponent();
            name = nameI;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Directory.Delete($"{Globals.dataPath}\\instance\\{name}", true);
            HomeScreen.reloadInstance("Default");
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
