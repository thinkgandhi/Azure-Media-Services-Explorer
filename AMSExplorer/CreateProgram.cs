﻿//----------------------------------------------------------------------- 
// <copyright file="CreateProgram.cs" company="Microsoft">Copyright (c) Microsoft Corporation. All rights reserved.</copyright> 
// <license>
// Azure Media Services Explorer Ver. 3.0
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
//  
// http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License. 
// </license> 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace AMSExplorer
{
    public partial class CreateProgram : Form
    {
        public string ChannelName;
        private CloudMediaContext _context;

        public string ProgramName
        {
            get { return textboxprogramname.Text; }
            set { textboxprogramname.Text = value; }
        }

        public string ProgramDescription
        {
            get { return textBoxDescription.Text; }
            set { textBoxDescription.Text = value; }
        }


        public TimeSpan archiveWindowLength
        {
            get
            {
                return new TimeSpan((int)numericUpDownArchiveDays.Value, (int)numericUpDownArchiveHours.Value, (int)numericUpDownArchiveMinutes.Value, 0); ;
            }
            set
            {
                numericUpDownArchiveDays.Value = value.Days;
                numericUpDownArchiveHours.Value = value.Hours;
                numericUpDownArchiveMinutes.Value = value.Minutes;
            }
        }

        public bool ProposeScaleUnit
        {
            set
            {
                checkBoxAddScaleUnit.Checked = value;
                checkBoxAddScaleUnit.Visible = value;
            }
        }

        public bool ScaleUnit
        {
            get
            {
                return checkBoxAddScaleUnit.Visible ? checkBoxAddScaleUnit.Checked : false;
            }
        }

        public bool IsReplica
        {
            get
            {
                return checkBoxReplica.Checked;
            }
        }

        public string ReplicaLocatorID
        {
            get { return labelLocatorID.Text; }

        }

        public string ReplicaManifestName
        {
            get { return labelManifestFile.Text; }

        }

        public string AssetName
        {
            get { return textBoxAssetName.Text; }
            set { textBoxAssetName.Text = value; }
        }

        public bool CreateLocator
        {
            get { return checkBoxCreateLocator.Checked; }
            set { checkBoxCreateLocator.Checked = value; }
        }

        public bool EnableDynEnc
        {
            get { return checkBoxDynEnc.Checked; }
            set { checkBoxDynEnc.Checked = value; }
        }

        public string StorageSelected
        {
            get { return ((Item)comboBoxStorage.SelectedItem).Value; }
        }

        public CreateProgram(CloudMediaContext context)
        {
            InitializeComponent();
            this.Icon = Bitmaps.Azure_Explorer_ico;
            _context = context;
        }

        private void CreateLocator_Load(object sender, EventArgs e)
        {
            this.Text = string.Format(this.Text, ChannelName);
            checkBoxCreateLocator.Text = string.Format(checkBoxCreateLocator.Text, Properties.Settings.Default.DefaultLocatorDurationDays);
            labelManifestFile.Text = string.Empty;
            labelLocatorID.Text = string.Empty;
            labelURLFileNameWarning.Text = string.Empty;

            foreach (var storage in _context.StorageAccounts)
            {
                comboBoxStorage.Items.Add(new Item(string.Format("{0} {1}", storage.Name, storage.IsDefault ? "(default)" : ""), storage.Name));
                if (storage.Name == _context.DefaultStorageAccount.Name) comboBoxStorage.SelectedIndex = comboBoxStorage.Items.Count - 1;
            }
        }

        private void checkBoxReplica_CheckedChanged(object sender, EventArgs e)
        {
            textBoxProgramSourceURL.Enabled = checkBoxReplica.Checked;
            if (checkBoxReplica.Checked)
            {
                checkBoxCreateLocator.Checked = true;
                checkBoxCreateLocator.Enabled = false;
            }
            else
            {
                checkBoxCreateLocator.Enabled = true;
            }
        }

        private void textBoxIProgramSourceURL_TextChanged(object sender, EventArgs e)
        {
            string filename = null;
            string locId = null;
            bool Error = false;
            string url = textBoxProgramSourceURL.Text;
            if (url.EndsWith("/manifest", StringComparison.OrdinalIgnoreCase))
            {
                url = url.ToLower().Replace("/manifest", string.Empty);
            }
            try
            {
                Uri myUri = new Uri(url);
                filename = System.IO.Path.GetFileNameWithoutExtension((myUri).LocalPath);
                locId = System.IO.Path.GetDirectoryName((myUri).LocalPath).Replace(@"\", "nb:lid:UUID:");
            }
            catch
            {
                Error = true;
                labelURLFileNameWarning.Text = "URL cannot be analyzed";
                labelManifestFile.Text = string.Empty;
                labelLocatorID.Text = string.Empty;
            }

            if (!Error)
            {
                labelURLFileNameWarning.Text = string.Empty;
                labelManifestFile.Text = filename;
                labelLocatorID.Text = locId;
            }
        }
    }
}
