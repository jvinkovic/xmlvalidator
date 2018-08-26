using System;
using System.Drawing;
using System.Windows.Forms;

namespace XmlValidator
{
    public partial class ValidationDetails : Form
    {
        private const string validLabel = "XML is VALID";
        private const string noXSDValidLabel = "XML is VALID but no schema present for further validation";
        private const string invalidLabel = "XML is INVALID";

        private ValidationData _validationData;

        public ValidationDetails(ValidationData validationData, bool xsdPresent = true)
        {
            InitializeComponent();
            lbErrors.Hide();
            _validationData = validationData;

            if (_validationData.Valid)
            {
                if (xsdPresent)
                {
                    lblValidity.Text = validLabel;
                }
                else
                {
                    lblValidity.Text = noXSDValidLabel;
                }
                lblValidity.ForeColor = Color.ForestGreen;
                this.Size = new Size(this.Width, 160);
            }
            else
            {
                lblValidity.Text = invalidLabel;
                lblValidity.ForeColor = Color.Red;
                this.Size = new Size(this.Width, 320);
                lbErrors.Show();
            }
        }

        private void ValidationDetails_Load(object sender, EventArgs e)
        {
            foreach (var ve in _validationData.Errors)
            {
                lbErrors.Items.Add(ve);
            }
        }
    }
}
