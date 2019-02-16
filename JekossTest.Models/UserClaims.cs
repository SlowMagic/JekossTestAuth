namespace JekossTest.Models
{
    public abstract class UserBaseClaims
    {
        public const string Id = "IdClaim";
        public const string FirstName = "FirstNameClaim";
        public const string LastName = "LastNameClaim";
        public static readonly string[] SystemClaimsToDrop =
        {
            "nbf",
            "exp",
            "aud",
            "iss"
        };
    }
}