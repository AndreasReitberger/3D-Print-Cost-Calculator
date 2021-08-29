using System.Runtime.Serialization;

namespace PrintCostCalculator3d.Models.Settings
{
    [DataContract]
    public class ActivationResponse
    {
        //"activated": true
        //"instance": 1473192358
        //"message": "2 out of 5 activations remaining"
        //"timestamp": 1473192358
        //"errorcode": 101
        //"errrormessage": Invalid license key...
        //"sig": "secret=null&activated=true&instance=1473192358&message=2 out of 5 activations remaining&timestamp=1473192358"

        [DataMember(Name = "activated")]
        public bool Activated { get; set; }

        [DataMember(Name = "instance")]
        public int Instance { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "timestamp")]
        public int Timestamp { get; set; }

        [DataMember(Name = "sig")]
        public string Sig { get; set; }

        [DataMember(Name = "code")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "error")]
        public string ErrorMessage { get; set; }
    }
    [DataContract]
    public class ActivationResponseWooSl
    {
        //"status": "error" | "success"
        //"status_code": s100 | s101.... => https://woosoftwarelicense.com/documentation/explain-api-status-codes/
        //"message": "Message"
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "status_code")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "message")]
        public string ErrorMessage { get; set; }
    }
    [DataContract]
    public class CodeVersionResponseWooSl
    {
        //"status": "error" | "success"
        //"status_code": s100 | s101.... => https://woosoftwarelicense.com/documentation/explain-api-status-codes/
        //"message": "Array of version information"
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "status_code")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "message")]
        public CodeVersionMessage versionMessage { get; set; }
    }
    [DataContract]
    public class CodeVersionMessage
    {
        //"name": the name
        //"version": the version
        //"last_updated": date of last update
        //"upgrade_notice": notice for this update
        //"author": the author
        //"tested": if tested or not
        //"requires": requires
        //"homepage": the homepage
        //"sections": sections => not used
        //"banners": banner images
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "last_updated")]
        public string LastUpdated { get; set; }

        [DataMember(Name = "upgrade_notice")]
        public string UpgradeNotice { get; set; }

        [DataMember(Name = "author")]
        public string Author { get; set; }

        [DataMember(Name = "tested")]
        public string Tested { get; set; }

        [DataMember(Name = "requires")]
        public string Requires { get; set; }

        [DataMember(Name = "homepage")]
        public string Homepage { get; set; }

        [DataMember(Name = "sections")]
        public string[] Sections { get; set; }

        [DataMember(Name = "banners")]
        public CodeVersionBanners Banners { get; set; }
    }
    [DataContract]
    public class CodeVersionBanners
    {
        //"low": banner url in low quality
        //"high": banner url in high quality

        [DataMember(Name = "low")]
        public string LowQualityBanner { get; set; }

        [DataMember(Name = "high")]
        public string HighQualityBanner { get; set; }
    }
    [DataContract]
    public class VerifyPurchaseCodeRespone
    {
        //"amount": the price of the item
        //"sold_At": date
        //"license": "Regular License" | "Extended License"
        //"support_amount": the price for support
        //"supported_until": date till support is available
        //"item": item information
        //"code": the purchase code
        [DataMember(Name = "amount")]
        public string Amount { get; set; }

        [DataMember(Name = "sold_At")]
        public string SoldAt { get; set; }

        [DataMember(Name = "license")]
        public string License { get; set; }

        [DataMember(Name = "support_amount")]
        public string SupportAmount { get; set; }

        [DataMember(Name = "supported_until")]
        public string SupportedUntil { get; set; }

        [DataMember(Name = "item")]
        public EnvatoItem Item { get; set; }

        [DataMember(Name = "code")]
        public string PurchaseCode { get; set; }
    }
    [DataContract]
    public class EnvatoItem
    {
        //"id": the numeric id of the item
        //"name": item name
        //"number_of_sales": sales of this item
        //"author_username": the author username
        //"author_url": the url to the author page
        //"url": item url
        //"updatedAt": last updated date of this item
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string ItemName { get; set; }

        [DataMember(Name = "number_of_sales")]
        public string Sales { get; set; }

        [DataMember(Name = "author_username")]
        public string Author { get; set; }

        [DataMember(Name = "author_url")]
        public string AuthorUrl { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "updatedAt")]
        public string UpdatedAt { get; set; }
    }

}
