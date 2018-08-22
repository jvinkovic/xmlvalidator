using FastColoredTextBoxNS;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace XmlValidator
{
    public partial class EditorForm : Form
    {
        private const string FORM_TITLE = "XML Creator & Validator";

        private Editor _editor;

        private ShowXSD xsdForm;

        public EditorForm()

        {
            InitializeComponent();
            this.Text = FORM_TITLE + "| Untitled";

            _editor = new Editor();

            editorBox.CustomAction += EditorBox_CustomAction;
        }

        /// <summary>
        /// easter egg for open XML and XSD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditorBox_CustomAction(object sender, CustomActionEventArgs e)
        {
            switch (e.Action)
            {
                case FCTBAction.CustomAction1:
                    openToolStripMenuItem_Click(null, null);
                    break;

                case FCTBAction.CustomAction2:
                    openXSDToolStripMenuItem_Click(null, null);
                    break;
            }
        }

        #region event handlers

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.DefaultExt = "xml";
                ofd.Filter = "XML files (*.xml)|*.xml";
                ofd.Multiselect = false;
                ofd.Title = "Open XML";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string content = _editor.OpenXML(ofd.FileName);
                    if (null == content)
                    {
                        MessageBox.Show("Error in opening file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        this.Text = FORM_TITLE + " | " + _editor.XmlName;
                        this.editorBox.Text = content;
                    }
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckIfIsChanged())
            {
                var xsd = _editor.GetXSD();
                _editor = new Editor();
                _editor.SetXSD(xsd);
            }

            this.Text = FORM_TITLE + "| Untitled";
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveXML();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _editor.SaveXMLAs(editorBox.Text);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveXML();
        }

        private void openXSDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.DefaultExt = "xsd";
                ofd.Filter = "XSD files (*.xsd)|*.xsd";
                ofd.Multiselect = false;
                ofd.Title = "Open XSD";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string xsdFilename = _editor.OpenXSD(ofd.FileName);

                    if (null == xsdFilename)
                    {
                        MessageBox.Show("Error in opening file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        tbCurrentXSD.Text = xsdFilename;
                    }
                }
            }
        }

        private void showXSDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null != xsdForm)
            {
                xsdForm.Dispose();
            }

            xsdForm = new ShowXSD(_editor?.GetXSD());
            xsdForm.Owner = this;

            var location = this.Location;
            location.X += this.Width - 20;
            xsdForm.Location = location;

            xsdForm.Show();
        }

        private void checkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckIt();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            CheckIt();
        }

        private void fctb_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            _editor?.FileChanged();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FontDialog fd = new FontDialog())
            {
                fd.FontMustExist = true;
                fd.ShowColor = true;
                fd.ShowEffects = true;
                fd.AllowVectorFonts = false;
                fd.AllowVerticalFonts = false;
                fd.FixedPitchOnly = true;

                fd.ShowApply = true;
                fd.Apply += applyFont;

                if (DialogResult.OK == fd.ShowDialog())
                {
                    editorBox.Font = fd.Font;
                }
            }
        }

        private void applyFont(object sender, EventArgs e)
        {
            editorBox.Font = ((FontDialog)sender).Font;
        }

        private void hotkeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new HotkeysEditorForm(editorBox.HotkeysMapping);
            if (form.ShowDialog() == DialogResult.OK)
                editorBox.HotkeysMapping = form.GetHotkeys();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editorBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editorBox.Redo();
        }

        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (false == CheckIfIsChanged())
            {
                e.Cancel = true;
            }
        }

        private void editorBox_KeyPressed(object sender, KeyPressEventArgs e)
        {
            // TODO handle key inputs and drop down suggestions
        }

        #endregion event handlers

        /// <summary>
        /// checks if text is changed
        /// </summary>
        /// <returns>true if it can be overwriten</returns>
        private bool CheckIfIsChanged()
        {
            if (_editor.IsFileChanged())
            {
                DialogResult result = MessageBox.Show("Do you want to save changes in " + _editor.XmlName + "?", "File modified", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None);

                if (result == DialogResult.Cancel)
                {
                    return false;
                }
                else if (result == DialogResult.Yes)
                {
                    if (!_editor.SaveXML(editorBox.Text))
                    {
                        MessageBox.Show("Error while saving file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return true;
        }

        private void SaveXML()
        {
            _editor.SaveXML(editorBox.Text);
        }

        private void CheckIt()
        {
            if (null == _editor.GetXSD())
            {
                MessageBox.Show("XSD schema file not selected", "No XSD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Task.Run(CheckXML);
        }

        private async Task CheckXML()
        {
            string xml = editorBox.Text;

            // heck if xml is empty
            if (string.IsNullOrEmpty(xml))
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("XML is empty", "Empty XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });
                }
                else
                {
                    MessageBox.Show("XML is empty", "Empty XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }

            // validate XML for basic structure only first
            try
            {
                new XmlDocument().LoadXml(xml);
            }
            catch (Exception)
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        // xml string is invalid
                        MessageBox.Show("XML is invalid", "Invalid XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                else
                {
                    // xml string is invalid
                    MessageBox.Show("XML is invalid", "Invalid XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return;
            }

            var validationResult = _editor.CheckXML(xml);

            using (var validDetailsForm = new ValidationDetails(validationResult))
            {
                validDetailsForm.ShowDialog();
            }
        }
    }
}
