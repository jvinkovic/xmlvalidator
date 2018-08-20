using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XmlValidator
{
    public partial class EditorForm : Form
    {
        private const string FORM_TITLE = "XML Creator & Validator";

        private Editor _editor;

        public EditorForm()

        {
            InitializeComponent();
            this.Text = FORM_TITLE;

            _editor = new Editor();
        }

        #region handlers

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.DefaultExt = "xml";
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
                _editor = new Editor();
            }
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

        private void xSDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.DefaultExt = "xsd";
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

        private void checkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckXML();
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

        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (false == CheckIfIsChanged())
            {
                e.Cancel = true;
            }
        }

        #endregion handlers

        /// <summary>
        /// checks if text is changed
        /// </summary>
        /// <returns>true if it can be overwriten</returns>
        public bool CheckIfIsChanged()
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

        public void SaveXML()
        {
            _editor.SaveXML(editorBox.Text);
        }

        public void CheckXML()
        {
            // TODO check handling
            _editor.CheckXML(editorBox.Text);
        }
    }
}
