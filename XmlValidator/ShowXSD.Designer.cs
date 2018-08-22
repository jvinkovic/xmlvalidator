namespace XmlValidator
{
    partial class ShowXSD
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowXSD));
            this.xsdContentBox = new FastColoredTextBoxNS.FastColoredTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.xsdContentBox)).BeginInit();
            this.SuspendLayout();
            // 
            // xsdContent
            // 
            this.xsdContentBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.xsdContentBox.AutoIndentCharsPatterns = "";
            this.xsdContentBox.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.xsdContentBox.BackBrush = null;
            this.xsdContentBox.CharHeight = 14;
            this.xsdContentBox.CharWidth = 8;
            this.xsdContentBox.CommentPrefix = null;
            this.xsdContentBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.xsdContentBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.xsdContentBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xsdContentBox.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.xsdContentBox.IsReplaceMode = false;
            this.xsdContentBox.Language = FastColoredTextBoxNS.Language.XML;
            this.xsdContentBox.LeftBracket = '<';
            this.xsdContentBox.LeftBracket2 = '(';
            this.xsdContentBox.Location = new System.Drawing.Point(0, 0);
            this.xsdContentBox.Name = "xsdContent";
            this.xsdContentBox.Paddings = new System.Windows.Forms.Padding(0);
            this.xsdContentBox.ReadOnly = true;
            this.xsdContentBox.RightBracket = '>';
            this.xsdContentBox.RightBracket2 = ')';
            this.xsdContentBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.xsdContentBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("xsdContent.ServiceColors")));
            this.xsdContentBox.Size = new System.Drawing.Size(616, 467);
            this.xsdContentBox.TabIndex = 0;
            this.xsdContentBox.Zoom = 100;
            // 
            // ShowXSD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 467);
            this.Controls.Add(this.xsdContentBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ShowXSD";
            this.ShowInTaskbar = false;
            this.Text = "XSD";
            ((System.ComponentModel.ISupportInitialize)(this.xsdContentBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox xsdContentBox;
    }
}