namespace XmlValidator
{
    partial class ValidationDetails
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
            this.lbErrors = new System.Windows.Forms.ListBox();
            this.lblValidity = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbErrors
            // 
            this.lbErrors.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbErrors.FormattingEnabled = true;
            this.lbErrors.HorizontalScrollbar = true;
            this.lbErrors.ItemHeight = 16;
            this.lbErrors.Location = new System.Drawing.Point(0, 117);
            this.lbErrors.Name = "lbErrors";
            this.lbErrors.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbErrors.Size = new System.Drawing.Size(520, 164);
            this.lbErrors.TabIndex = 1;
            // 
            // lblValidity
            // 
            this.lblValidity.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblValidity.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValidity.Location = new System.Drawing.Point(0, 0);
            this.lblValidity.Name = "lblValidity";
            this.lblValidity.Size = new System.Drawing.Size(520, 113);
            this.lblValidity.TabIndex = 2;
            this.lblValidity.Text = "XML is VALID";
            this.lblValidity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ValidationDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 281);
            this.Controls.Add(this.lblValidity);
            this.Controls.Add(this.lbErrors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ValidationDetails";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Validation Details";
            this.Load += new System.EventHandler(this.ValidationDetails_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbErrors;
        private System.Windows.Forms.Label lblValidity;
    }
}