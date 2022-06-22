using System;
namespace AmazonAtelierSPAPI.SellingPartnerAPIAA
{
    public class SigningDateHelper : IDateHelper
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
