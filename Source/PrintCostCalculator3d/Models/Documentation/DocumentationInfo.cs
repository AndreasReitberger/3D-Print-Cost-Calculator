using PrintCostCalculator3d.Models.Settings;
using System.Collections.Generic;


namespace PrintCostCalculator3d.Models.Documentation
{
    public class DocumentationInfo
    {
        public DocumentationIdentifier Identifier { get; set; }
        public string Path { get; set; }

        public DocumentationInfo(DocumentationIdentifier identifier, string path)
        {
            Identifier = identifier;
            Path = path;
        }
    }
}
