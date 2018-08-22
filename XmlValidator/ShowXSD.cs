using System.Windows.Forms;

namespace XmlValidator
{
    public partial class ShowXSD : Form
    {
        public ShowXSD(XSDData xsdData)
        {
            InitializeComponent();

            this.Text += " | " + xsdData?.Name;
            xsdContentBox.Text = xsdData?.Content;
        }
    }
}
