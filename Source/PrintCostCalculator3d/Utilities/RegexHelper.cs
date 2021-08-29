﻿using System.Diagnostics.CodeAnalysis;

namespace PrintCostCalculator3d.Utilities
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class RegexHelper
    {
        // Repetier Server Pro API Key
        public const string OctoPrintApiKey = @"([A-Z]|[0-9])*";
        
        // Repetier Server Pro API Key
        public const string RepetierServerProApiKey = @"(\w{8}-\w{4}-\w{4}-\w{4}-\w{12})";
        public const string Fqdn = @"(?=^.{4,253}$)(^((?!-)[a-zA-Z0-9-]{1,63}(?<!-)\.)+[a-zA-Z]{2,63}$)";

        // Match IPv4-Address like 192.168.178.1
        const string IPv4AddressValues = @"(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])";
        public const string IPv4AddressRegex = "^" + IPv4AddressValues + "$";

        // Match IPv6-Address
        public const string IPv6AddressRegex = @"(?:^|(?<=\s))(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))(?=\s|$)";

        // Match IPv4-Address Range like 192.168.178.1-192.168.178.127
        public const string IPv4AddressRangeRegex = "^" + IPv4AddressValues + "-" + IPv4AddressValues + "$";

        // Match a MAC-Address 000000000000 00:00:00:00:00:00, 00-00-00-00-00-00-00 or 0000.0000.0000
        public const string MACAddressRegex = @"^^[A-Fa-f0-9]{12}$|^[A-Fa-f0-9]{2}(:|-){1}[A-Fa-f0-9]{2}(:|-){1}[A-Fa-f0-9]{2}(:|-){1}[A-Fa-f0-9]{2}(:|-){1}[A-Fa-f0-9]{2}(:|-){1}[A-Fa-f0-9]{2}$|^[A-Fa-f0-9]{4}.[A-Fa-f0-9]{4}.[A-Fa-f0-9]{4}$$";

        // Matche the first 3 bytes of a MAC-Address 000000, 00:00:00, 00-00-00
        public const string MACAddressFirst3BytesRegex = @"^[A-Fa-f0-9]{6}$|^[A-Fa-f0-9]{2}(:|-){1}[A-Fa-f0-9]{2}(:|-){1}[A-Fa-f0-9]{2}$|^[A-Fa-f0-9]{4}.[A-Fa-f0-9]{2}$";

        // subnetmask / cidr values
        const string SubnetmaskValues = @"(((255\.){3}(255|254|252|248|240|224|192|128|0+))|((255\.){2}(255|254|252|248|240|224|192|128|0+)\.0)|((255\.)(255|254|252|248|240|224|192|128|0+)(\.0+){2})|((255|254|252|248|240|224|192|128|0+)(\.0+){3}))";
        const string CidrRegex = @"([1-9]|[1-2][0-9]|3[0-2])";

        // Match a Subnetmask like 255.255.255.0
        public const string SubnetmaskRegex = @"^" + SubnetmaskValues + @"$";

        // Match a subnet from 192.168.178.0/1 to 192.168.178.0/32
        public const string IPv4AddressCidrRegex = @"^" + IPv4AddressValues + @"\/" + CidrRegex + @"$";

        // Match a subnet from 192.168.178.0/0 to 192.168.178.0/32
        public const string SubnetCalculatorIPv4AddressCidrRegex = @"^" + IPv4AddressValues + @"\/" + CidrRegex + @"$";

        // Match a subnet from 192.168.178.0/192.0.0.0 to 192.168.178.0/255.255.255.255
        public const string IPv4AddressSubnetmaskRegex = @"^" + IPv4AddressValues + @"\/" + SubnetmaskValues + @"$";

        // Match a subnet from 192.168.178.0/0.0.0.0 to 192.168.178.0/255.255.255.255
        public const string SubnetCalculatorIPv4AddressSubnetmaskRegex = "^" + IPv4AddressValues + @"\/" + SubnetmaskValues + @"$";

        // Match a subnet like 2001:0db8::/64
        public const string IPv6AddressCidrRegex = @"^s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:)))(%.+)?s*(\/([0-9]|[1-9][0-9]|1[0-1][0-9]|12[0-8])){1}?$";

        // Match a range like [0-255], [0,2,4] and [2,4-6]
        public const string SpecialRangeRegex = @"\[((?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)|((?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)-(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)))([,]((?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)|((?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)-(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))))*\]";

        // Match a IPv4-Address like 192.168.[50-100].1
        public const string IPv4AddressSpecialRangeRegex = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|" + SpecialRangeRegex + @")\.){3}((?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)|" + SpecialRangeRegex + @")$";

        // hostname values
        const string HostnameValues = @"(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])";

        // Hostname regex
        public const string HostnameRegex = @"^" + HostnameValues + @"$";

        // Match a hostname with cidr like server-01.example.com/24
        public const string HostnameCidrRegex = @"^" + HostnameValues + @"\/" + CidrRegex + @"$";

        // Match a hostname with subnetmask like server-01.example.com/255.255.255.0
        public const string HostnameSubnetmaskRegex = @"^" + HostnameValues + @"\/" + SubnetmaskValues + @"$";

        // Match a domain local.example.com
        public const string DomainRegex = @"^(?!:\/\/)([a-zA-Z0-9-_]+\.)*[a-zA-Z0-9][a-zA-Z0-9-_]+\.[a-zA-Z]{2,11}?$";

        // Match any filepath --> https://www.codeproject.com/Tips/216238/Regular-Expression-to-Validate-File-Path-and-Exten
        public const string FilePath = @"^(?:[\w]\:|\\)(\\[a-z_\-\s0-9\.]+)+\.[a-zA-z0-9]{1,4}$";
    }
}
