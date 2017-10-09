namespace Mbed.RPC.Serial
{
    partial class SerialRPCForm
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
            this.startButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.led4_chkBox = new System.Windows.Forms.CheckBox();
            this.led3_chkBox = new System.Windows.Forms.CheckBox();
            this.led2_chkBox = new System.Windows.Forms.CheckBox();
            this.led1_chkBox = new System.Windows.Forms.CheckBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.serialRPCBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.serialComboBox = new System.Windows.Forms.ComboBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(33, 321);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Blink LEDs";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Serial Port:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.led4_chkBox);
            this.groupBox1.Controls.Add(this.led3_chkBox);
            this.groupBox1.Controls.Add(this.led2_chkBox);
            this.groupBox1.Controls.Add(this.led1_chkBox);
            this.groupBox1.Location = new System.Drawing.Point(32, 91);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(201, 215);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "LED On/Off";
            // 
            // led4_chkBox
            // 
            this.led4_chkBox.AutoSize = true;
            this.led4_chkBox.Location = new System.Drawing.Point(6, 167);
            this.led4_chkBox.Name = "led4_chkBox";
            this.led4_chkBox.Padding = new System.Windows.Forms.Padding(5);
            this.led4_chkBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.led4_chkBox.Size = new System.Drawing.Size(78, 27);
            this.led4_chkBox.TabIndex = 3;
            this.led4_chkBox.Text = "    LED 4";
            this.led4_chkBox.UseVisualStyleBackColor = true;
            this.led4_chkBox.CheckedChanged += new System.EventHandler(this.led4_chkBox_CheckedChanged);
            // 
            // led3_chkBox
            // 
            this.led3_chkBox.AutoSize = true;
            this.led3_chkBox.Location = new System.Drawing.Point(6, 118);
            this.led3_chkBox.Name = "led3_chkBox";
            this.led3_chkBox.Padding = new System.Windows.Forms.Padding(5);
            this.led3_chkBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.led3_chkBox.Size = new System.Drawing.Size(78, 27);
            this.led3_chkBox.TabIndex = 2;
            this.led3_chkBox.Text = "    LED 3";
            this.led3_chkBox.UseVisualStyleBackColor = true;
            this.led3_chkBox.CheckedChanged += new System.EventHandler(this.led3_chkBox_CheckedChanged);
            // 
            // led2_chkBox
            // 
            this.led2_chkBox.AutoSize = true;
            this.led2_chkBox.Location = new System.Drawing.Point(6, 73);
            this.led2_chkBox.Name = "led2_chkBox";
            this.led2_chkBox.Padding = new System.Windows.Forms.Padding(5);
            this.led2_chkBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.led2_chkBox.Size = new System.Drawing.Size(78, 27);
            this.led2_chkBox.TabIndex = 1;
            this.led2_chkBox.Text = "    LED 2";
            this.led2_chkBox.UseVisualStyleBackColor = true;
            this.led2_chkBox.CheckedChanged += new System.EventHandler(this.led2_chkBox_CheckedChanged);
            // 
            // led1_chkBox
            // 
            this.led1_chkBox.AutoSize = true;
            this.led1_chkBox.Location = new System.Drawing.Point(6, 29);
            this.led1_chkBox.Name = "led1_chkBox";
            this.led1_chkBox.Padding = new System.Windows.Forms.Padding(5);
            this.led1_chkBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.led1_chkBox.Size = new System.Drawing.Size(78, 27);
            this.led1_chkBox.TabIndex = 0;
            this.led1_chkBox.Text = "    LED 1";
            this.led1_chkBox.UseVisualStyleBackColor = true;
            this.led1_chkBox.CheckedChanged += new System.EventHandler(this.led1_chkBox_CheckedChanged);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(158, 321);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "Exit";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // serialComboBox
            // 
            this.serialComboBox.FormattingEnabled = true;
            this.serialComboBox.Location = new System.Drawing.Point(99, 43);
            this.serialComboBox.Name = "serialComboBox";
            this.serialComboBox.Size = new System.Drawing.Size(87, 21);
            this.serialComboBox.TabIndex = 5;
            this.serialComboBox.SelectedIndexChanged += new System.EventHandler(this.serialComboBox_SelectedIndexChanged);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.statusLabel.Location = new System.Drawing.Point(12, 364);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Padding = new System.Windows.Forms.Padding(1);
            this.statusLabel.Size = new System.Drawing.Size(85, 17);
            this.statusLabel.TabIndex = 6;
            this.statusLabel.Text = "Not connected!";
            // 
            // SerialRPCForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 389);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.serialComboBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startButton);
            this.Name = "SerialRPCForm";
            this.Text = "SerialRPC";
            this.Load += new System.EventHandler(this.SerialRPCForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox led4_chkBox;
        private System.Windows.Forms.CheckBox led3_chkBox;
        private System.Windows.Forms.CheckBox led2_chkBox;
        private System.Windows.Forms.CheckBox led1_chkBox;
        private System.Windows.Forms.Button stopButton;
        private System.ComponentModel.BackgroundWorker serialRPCBackgroundWorker;
        private System.Windows.Forms.ComboBox serialComboBox;
        private System.Windows.Forms.Label statusLabel;
    }
}

