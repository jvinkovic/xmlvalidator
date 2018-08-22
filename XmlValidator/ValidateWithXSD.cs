using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XmlValidator
{
    public class ValidateWithXSD
    {
        private string _xmlContent;
        private XSDData _xsdData;

        private ValidationData validationResult = new ValidationData();

        public ValidateWithXSD(ref string xml, XSDData xsd)
        {
            _xmlContent = xml;
            _xsdData = xsd;
        }

        public async Task<ValidationData> Validate()
        {
            validationResult = new ValidationData();

            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.ProcessInlineSchema
                                        | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.IgnoreComments = true;

            // Create the XmlDocument object.
            XDocument doc = XDocument.Parse(_xmlContent);

            var schemas = new XmlSchemaSet();
            using (var stringReader = new StringReader(_xsdData?.Content))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    schemas.Add("", xmlReader);

                    doc.Validate(schemas, ValidationErrorsHandler);
                }
            }

            return validationResult;
        }

        // Save any warnings or errors.
        private void ValidationErrorsHandler(object sender, ValidationEventArgs args)
        {
            validationResult.Valid = false;

            switch (args.Severity)
            {
                case XmlSeverityType.Error:
                    validationResult.Errors.Add("ERROR: " + args.Message);
                    break;

                case XmlSeverityType.Warning:
                    validationResult.Errors.Add("WARNING: " + args.Message);
                    break;
            }
        }
    }
}
