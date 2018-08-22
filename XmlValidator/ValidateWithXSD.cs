using System;
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

        public ValidationData Validate()
        {
            validationResult = new ValidationData();

            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.ProcessInlineSchema
                                        | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += ValidationErrorsHandler;

            var schemas = new XmlSchemaSet();

            try
            {
                schemas.Add(null, _xsdData?.Path); // null - using targetNamespace from the schema
                schemas.Compile(); // compile all into one logical schema

                settings.Schemas = schemas;
            }
            catch (Exception ex)
            {
                validationResult.Valid = false;
                validationResult.Errors.Add("ERROR: XSD SCHEMA INVALID - " + ex.Message);
            }

            try
            {
                using (var stringXmlReader = new StringReader(_xmlContent))
                {
                    using (var xmlReader = XmlReader.Create(stringXmlReader, settings))
                    {
                        while (xmlReader.Read()) ;
                    }
                }
            }
            catch (Exception ex)
            { }

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
