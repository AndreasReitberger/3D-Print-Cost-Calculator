﻿using WpfFramework.Models.Settings;
using System.Collections.Generic;


namespace WpfFramework.Models.Documentation
{
    public class DocumentationInfo
    {
        public DocumentationIdentifier Identifier { get; set; }
        public string Path { get; set; }
        public List<LocalizationInfo> Localizations { get; set; }

        public DocumentationInfo(DocumentationIdentifier identifier, string path, List<LocalizationInfo> localizations)
        {
            Identifier = identifier;
            Path = path;
            Localizations = localizations;
        }
    }
}
