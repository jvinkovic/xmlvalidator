﻿using FastColoredTextBoxNS;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace XmlValidator
{
    public partial class EditorForm : Form
    {
        private const string FORM_TITLE = "XML Creator & Validator";

        private Editor _editor;

        private ShowXSD xsdForm;

        private Keys LastKeyPressed = Keys.A;

        public EditorForm()

        {
            InitializeComponent();
            this.Text = FORM_TITLE + " | Untitled";

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

            this.Text = FORM_TITLE + " | Untitled";
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

        private void omitNamespaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckIt(true);
        }

        private void btnCheck_MouseHover(object sender, EventArgs e)
        {
            tooltip.SetToolTip(sender as Control, "Validation with namespace checks");
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            CheckIt();
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

        private void fctb_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            _editor?.FileChanged();

            // if no XSD is selected
            if (null == _editor?.CurrentNode)
            {
                return;
            }

            _editor.ResetCurrentNode();
            var len = editorBox.SelectionStart;
            for (int i = 0; i < len - 1; i++)
            {
                var c = editorBox.Text[i];
                CharInputed(c, false, i);
            }

            if (len > 0)
            {
                bool notDeleting = LastKeyPressed != Keys.Back && LastKeyPressed != Keys.Delete;
                CharInputed(editorBox.Text[len - 1], notDeleting, len - 1);
            }
        }

        private void CharInputed(char keyChar, bool toOpen = true, int lastPostion = -1)
        {
            contextMenu.Items.Clear();

            if (keyChar == '<')
            {
                _editor.CurrentNodeOpening();

                var elements = _editor.CurrentNodeElements();
                if (null != elements)
                {
                    foreach (var item in elements)
                    {
                        contextMenu.Items.Add(item);
                    }
                }
            }
            else if (keyChar == ' ')
            {
                if (_editor.CurrentNodeStatus == NodeStatus.Opening)
                {
                    // get node name
                    var nodeName = GetCurrentNodeName(lastPostion);

                    _editor.CurrentNodeOpen(nodeName);
                }

                if (_editor.CurrentNodeStatus == NodeStatus.Open)
                {
                    var attributes = _editor.CurrentNodeAttributes();
                    if (null != attributes)
                    {
                        foreach (var item in attributes)
                        {
                            var tsItem = new ToolStripMenuItem(item);
                            tsItem.Tag = Editor.ATTR_TAG;
                            contextMenu.Items.Add(tsItem);
                        }
                    }
                }
            }
            else if (keyChar == '>' && _editor.CurrentNodeStatus == NodeStatus.Opening)
            {
                // get node name
                var nodeName = GetCurrentNodeName(lastPostion);

                _editor.CurrentNodeEnd(nodeName);

                toOpen = false;
            }
            else if (keyChar == '/')
            {
                if (null == _editor.CurrentNode.Parent)
                {
                    return;
                }

                var previousChar = ' ';
                if (lastPostion > 0)
                {
                    previousChar = editorBox.Text[lastPostion - 1];
                }
                if (previousChar == '<')
                {
                    _editor.CurrentNodeClosing();
                    contextMenu.Items.Add(_editor.CurrentNode.Name + ">");
                }
            }
            else if (keyChar == '>')
            {
                if (_editor.CurrentNodeStatus == NodeStatus.Closing)
                {
                    _editor.CurrentNodeClose();
                }
                else if (_editor.CurrentNodeStatus == NodeStatus.Open)
                {
                    // get node name
                    var nodeName = GetCurrentNodeName(lastPostion);

                    _editor.CurrentNodeEnd(nodeName);
                }

                toOpen = false;
            }
            else
            {
                // nothing
                return;
            }

            if (toOpen)
            {
                // get location and show the menu
                var location = editorBox.PositionToPoint(editorBox.SelectionStart);
                location.X += 3;
                location.Y += editorBox.Font.Height * 3;

                contextMenu.Show(this, location);
            }
        }

        private string GetCurrentNodeName(int lastPosition = -1)
        {
            var start = 0;
            if (lastPosition > -1)
            {
                start = editorBox.Text.Substring(0, lastPosition).LastIndexOf("<");
            }
            else
            {
                start = editorBox.Text.LastIndexOf("<");
            }
            StringBuilder nodeName = new StringBuilder();
            char currChar = editorBox.Text[start];
            int i = start;
            do
            {
                i++;
                if (i < editorBox.Text.Length)
                {
                    currChar = editorBox.Text[i];
                    nodeName.Append(currChar);
                }
                else
                {
                    break;
                }
            } while (currChar != ' ' && currChar != '>');

            var name = nodeName.ToString().TrimEnd('>', ' ');
            return name;
        }

        private void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var text = e.ClickedItem.Text;
            var cursorPosition = editorBox.SelectionStart + text.Length;

            // if it is attribute it shpulh have tag Editor.ATTR_TAG
            if ((string)e.ClickedItem.Tag == Editor.ATTR_TAG)
            {
                // remove data type and add '='
                text = text.Substring(0, text.LastIndexOf(Editor.ATTR_SEPARATOR)) + "=\"\"";

                cursorPosition = editorBox.SelectionStart + text.Length - 1;
            }

            editorBox.Text = editorBox.Text.Insert(editorBox.SelectionStart, text);

            // put cursor after insert or one before insert end if attribute - before closing '"'
            editorBox.SelectionStart = cursorPosition;
        }

        private void editorBox_KeyDown(object sender, KeyEventArgs e)
        {
            LastKeyPressed = e.KeyCode;
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

        private void CheckIt(bool omitNamespace = false)
        {
            if (null == _editor.GetXSD())
            {
                MessageBox.Show("XSD schema file not selected", "No XSD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Task.Run(() => CheckXML(omitNamespace));
        }

        private void CheckXML(bool omitNamespace = false)
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

            ValidationData validationResult = new ValidationData();
            if (null != _editor.GetXSD())
            {
                validationResult = _editor.CheckXML(xml, omitNamespace);
            }

            using (var validDetailsForm = new ValidationDetails(validationResult))
            {
                validDetailsForm.ShowDialog();
            }
        }
    }
}
