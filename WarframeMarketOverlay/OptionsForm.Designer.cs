namespace WarframeMarketOverlay
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.groupBoxHotkey = new System.Windows.Forms.GroupBox();
            this.labelNote2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelNote1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxKey = new System.Windows.Forms.ComboBox();
            this.checkBoxWin = new System.Windows.Forms.CheckBox();
            this.checkBoxShift = new System.Windows.Forms.CheckBox();
            this.checkBoxCtrl = new System.Windows.Forms.CheckBox();
            this.checkBoxAlt = new System.Windows.Forms.CheckBox();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.buttonConfirm = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.labelUpdateDone = new System.Windows.Forms.Label();
            this.checkBoxStartup = new System.Windows.Forms.CheckBox();
            this.groupBoxHotkey.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxHotkey
            // 
            this.groupBoxHotkey.Controls.Add(this.labelNote2);
            this.groupBoxHotkey.Controls.Add(this.label3);
            this.groupBoxHotkey.Controls.Add(this.labelNote1);
            this.groupBoxHotkey.Controls.Add(this.label1);
            this.groupBoxHotkey.Controls.Add(this.comboBoxKey);
            this.groupBoxHotkey.Controls.Add(this.checkBoxWin);
            this.groupBoxHotkey.Controls.Add(this.checkBoxShift);
            this.groupBoxHotkey.Controls.Add(this.checkBoxCtrl);
            this.groupBoxHotkey.Controls.Add(this.checkBoxAlt);
            this.groupBoxHotkey.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBoxHotkey.Location = new System.Drawing.Point(12, 12);
            this.groupBoxHotkey.Name = "groupBoxHotkey";
            this.groupBoxHotkey.Size = new System.Drawing.Size(473, 237);
            this.groupBoxHotkey.TabIndex = 0;
            this.groupBoxHotkey.TabStop = false;
            this.groupBoxHotkey.Text = "Hotkey Settings";
            // 
            // labelNote2
            // 
            this.labelNote2.AutoSize = true;
            this.labelNote2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelNote2.Location = new System.Drawing.Point(24, 201);
            this.labelNote2.Name = "labelNote2";
            this.labelNote2.Size = new System.Drawing.Size(371, 16);
            this.labelNote2.TabIndex = 8;
            this.labelNote2.Text = "* - To confirm the changes, you need to restart the application.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(345, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Key:";
            // 
            // labelNote1
            // 
            this.labelNote1.AutoSize = true;
            this.labelNote1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelNote1.Location = new System.Drawing.Point(24, 142);
            this.labelNote1.MaximumSize = new System.Drawing.Size(371, 0);
            this.labelNote1.Name = "labelNote1";
            this.labelNote1.Size = new System.Drawing.Size(365, 48);
            this.labelNote1.TabIndex = 6;
            this.labelNote1.Text = "* - When you are registering a hotkey, be careful that you don\'t overwrite a hotk" +
    "ey that has already been registered by a different application.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Modifiers:";
            // 
            // comboBoxKey
            // 
            this.comboBoxKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKey.FormattingEnabled = true;
            this.comboBoxKey.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.comboBoxKey.Location = new System.Drawing.Point(342, 69);
            this.comboBoxKey.Name = "comboBoxKey";
            this.comboBoxKey.Size = new System.Drawing.Size(53, 28);
            this.comboBoxKey.TabIndex = 4;
            // 
            // checkBoxWin
            // 
            this.checkBoxWin.AutoSize = true;
            this.checkBoxWin.Location = new System.Drawing.Point(173, 90);
            this.checkBoxWin.Name = "checkBoxWin";
            this.checkBoxWin.Size = new System.Drawing.Size(95, 24);
            this.checkBoxWin.TabIndex = 3;
            this.checkBoxWin.Text = "WIN KEY";
            this.checkBoxWin.UseVisualStyleBackColor = true;
            // 
            // checkBoxShift
            // 
            this.checkBoxShift.AutoSize = true;
            this.checkBoxShift.Location = new System.Drawing.Point(173, 60);
            this.checkBoxShift.Name = "checkBoxShift";
            this.checkBoxShift.Size = new System.Drawing.Size(75, 24);
            this.checkBoxShift.TabIndex = 2;
            this.checkBoxShift.Text = "SHIFT";
            this.checkBoxShift.UseVisualStyleBackColor = true;
            // 
            // checkBoxCtrl
            // 
            this.checkBoxCtrl.AutoSize = true;
            this.checkBoxCtrl.Location = new System.Drawing.Point(49, 90);
            this.checkBoxCtrl.Name = "checkBoxCtrl";
            this.checkBoxCtrl.Size = new System.Drawing.Size(69, 24);
            this.checkBoxCtrl.TabIndex = 1;
            this.checkBoxCtrl.Text = "CTRL";
            this.checkBoxCtrl.UseVisualStyleBackColor = true;
            // 
            // checkBoxAlt
            // 
            this.checkBoxAlt.AutoSize = true;
            this.checkBoxAlt.Location = new System.Drawing.Point(49, 60);
            this.checkBoxAlt.Name = "checkBoxAlt";
            this.checkBoxAlt.Size = new System.Drawing.Size(57, 24);
            this.checkBoxAlt.TabIndex = 0;
            this.checkBoxAlt.Text = "ALT";
            this.checkBoxAlt.UseVisualStyleBackColor = true;
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.checkBoxStartup);
            this.groupBoxSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 264);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(473, 113);
            this.groupBoxSettings.TabIndex = 1;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Application Settings";
            // 
            // buttonConfirm
            // 
            this.buttonConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonConfirm.Location = new System.Drawing.Point(251, 412);
            this.buttonConfirm.Name = "buttonConfirm";
            this.buttonConfirm.Size = new System.Drawing.Size(114, 30);
            this.buttonConfirm.TabIndex = 2;
            this.buttonConfirm.Text = "Confirm";
            this.buttonConfirm.UseVisualStyleBackColor = true;
            this.buttonConfirm.Click += new System.EventHandler(this.ButtonConfirm_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonCancel.Location = new System.Drawing.Point(371, 412);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(114, 30);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonUpdate.Location = new System.Drawing.Point(12, 412);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(150, 30);
            this.buttonUpdate.TabIndex = 4;
            this.buttonUpdate.Text = "Update Item List";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // labelUpdateDone
            // 
            this.labelUpdateDone.AutoSize = true;
            this.labelUpdateDone.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelUpdateDone.Location = new System.Drawing.Point(182, 419);
            this.labelUpdateDone.Name = "labelUpdateDone";
            this.labelUpdateDone.Size = new System.Drawing.Size(0, 16);
            this.labelUpdateDone.TabIndex = 5;
            // 
            // checkBoxStartup
            // 
            this.checkBoxStartup.AutoSize = true;
            this.checkBoxStartup.Location = new System.Drawing.Point(49, 36);
            this.checkBoxStartup.Name = "checkBoxStartup";
            this.checkBoxStartup.Size = new System.Drawing.Size(198, 24);
            this.checkBoxStartup.TabIndex = 0;
            this.checkBoxStartup.Text = "Run at Windows startup";
            this.checkBoxStartup.UseVisualStyleBackColor = true;
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.buttonConfirm;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(497, 454);
            this.Controls.Add(this.labelUpdateDone);
            this.Controls.Add(this.buttonUpdate);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonConfirm);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.groupBoxHotkey);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.Text = "OptionsForm";
            this.groupBoxHotkey.ResumeLayout(false);
            this.groupBoxHotkey.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxHotkey;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Button buttonConfirm;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxWin;
        private System.Windows.Forms.CheckBox checkBoxShift;
        private System.Windows.Forms.CheckBox checkBoxCtrl;
        private System.Windows.Forms.CheckBox checkBoxAlt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelNote1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxKey;
        private System.Windows.Forms.Label labelNote2;
        private System.Windows.Forms.Button buttonUpdate;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.Windows.Forms.Label labelUpdateDone;
        private System.Windows.Forms.CheckBox checkBoxStartup;
    }
}