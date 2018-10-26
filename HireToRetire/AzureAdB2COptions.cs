using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HireToRetire
{
    public class AzureAdB2COptions
    {
        public const string PolicyAuthenticationProperty = "Policy";

        public AzureAdB2COptions()
        {
            AzureAdB2CInstance = "https://CapApps.b2clogin.com";
            // https://CapApps.b2clogin.com/CapApps.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_SignUpIn

            // https://login.microsoftonline.com/CapApps.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_SignUpIn
        }

        public string ClientId { get; set; }
        public string AzureAdB2CInstance { get; set; }
        public string Tenant { get; set; }
        public string SignUpSignInPolicyId { get; set; }
        public string SignInPolicyId { get; set; }
        public string SignUpPolicyId { get; set; }
        public string ResetPasswordPolicyId { get; set; }
        public string EditProfilePolicyId { get; set; }
        public string RedirectUri { get; set; }

        public string DefaultPolicy => SignUpSignInPolicyId;
        public string Authority => $"{AzureAdB2CInstance}/{Tenant}/{DefaultPolicy}/v2.0";

        public string ClientSecret { get; set; }
        public string ApiUrl { get; set; }
        public string ApiScopes { get; set; }
    }
}
