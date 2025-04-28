namespace MultiServer1._0
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            StartServerButton = new Button();
            listBoxMessages = new ListBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // StartServerButton
            // 
            StartServerButton.Dock = DockStyle.Left;
            StartServerButton.Location = new Point(0, 0);
            StartServerButton.Name = "StartServerButton";
            StartServerButton.Size = new Size(129, 450);
            StartServerButton.TabIndex = 0;
            StartServerButton.Text = "Start";
            StartServerButton.UseVisualStyleBackColor = true;
            StartServerButton.Click += StartServerButton_Click;
            // 
            // listBoxMessages
            // 
            listBoxMessages.Dock = DockStyle.Right;
            listBoxMessages.FormattingEnabled = true;
            listBoxMessages.ItemHeight = 15;
            listBoxMessages.Location = new Point(280, 0);
            listBoxMessages.Name = "listBoxMessages";
            listBoxMessages.Size = new Size(520, 450);
            listBoxMessages.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(135, 167);
            button1.Name = "button1";
            button1.Size = new Size(129, 21);
            button1.TabIndex = 2;
            button1.Text = "Close";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(listBoxMessages);
            Controls.Add(StartServerButton);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button StartServerButton;
        private ListBox listBoxMessages;
        private Button button1;
    }
}
