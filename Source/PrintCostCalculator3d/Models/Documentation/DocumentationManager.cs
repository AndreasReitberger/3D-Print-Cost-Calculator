using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;


namespace PrintCostCalculator3d.Models.Documentation
{
    public static class DocumentationManager
    {
        public const string DocumentationBaseUrl = @"https://andreas-reitberger.de/en/docs/3d-druckkosten-kalkulator/";

        public static List<DocumentationInfo> List => new List<DocumentationInfo>
        {
            new DocumentationInfo(DocumentationIdentifier.MachineHourRateCalculator, @"module/maschinenstundensatz-berechnen"),
            new DocumentationInfo(DocumentationIdentifier.RepetierServerProDashboard, @"module/repetier-server-pro-dashboard/"),
            new DocumentationInfo(DocumentationIdentifier.OctoPrintDashboard, @"module/octoprint-dashboard/"),
            new DocumentationInfo(DocumentationIdentifier._3dPrintCostCalculation, @"module/druckkosten-kalkulator/"),
            new DocumentationInfo(DocumentationIdentifier._3dPrinters, @"module/3d-drucker/"),
            new DocumentationInfo(DocumentationIdentifier._3dMaterials, @"module/materialien/"),
            new DocumentationInfo(DocumentationIdentifier.AdditionalWorkSteps, @"module/zusaetzliche-arbeitsschritte-verwalten/"),

            //Dialogs
            new DocumentationInfo(DocumentationIdentifier.SlicerDialog, @"slicer/slicer-integrieren/"),
            new DocumentationInfo(DocumentationIdentifier.GcodeParser, @"parser/gcode-parser/"),

        };

        // Get localized documentation url (if available), else return the english page
        public static string CreateUrl(DocumentationIdentifier documentationIdentifier)
        {
            var info = List.FirstOrDefault(x => x.Identifier == documentationIdentifier);

            var url = DocumentationBaseUrl;

            if (info != null)
                url += info.Path;

            return url;
        }

        public static void OpenDocumentation(DocumentationIdentifier documentationIdentifier)
        {
            Process.Start(CreateUrl(documentationIdentifier));
        }

        #region ICommands & Actions
        public static ICommand OpenDocumentationCommand => new RelayCommand(OpenDocumentationAction);

        static void OpenDocumentationAction(object documentationIdentifier)
        {
            if (documentationIdentifier != null)
                OpenDocumentation((DocumentationIdentifier)documentationIdentifier);
        }

        public static DocumentationIdentifier GetIdentifierByAppliactionName(ApplicationName name)
        {
            switch (name)
            {

                case ApplicationName.None:
                    return DocumentationIdentifier.Default;
                case ApplicationName._3dPrintingPrinter:
                    return DocumentationIdentifier._3dPrinters;
                case ApplicationName._3dPrintingMaterial:
                    return DocumentationIdentifier._3dMaterials;
                default:
                    return DocumentationIdentifier.Default;
            }
        }
        #endregion
    }
}
