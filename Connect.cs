using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmazonAtelierSPAPI.SellingPartnerAPIAA;
using Newtonsoft.Json;
using RestSharp;
using Amazon.SecurityToken.Model;
using Amazon.Runtime;
using Amazon.SecurityToken;
using System.Collections;

namespace AmazonAtelierSPAPI.Connect
 {
    public class ConnectToAmazon
    {
        public string resp { get; set; }
        // params static
        private string url_token = "https://api.amazon.com/auth/o2/token";
        private string url_host = "https://sellingpartnerapi-eu.amazon.com";

        public ConnectToAmazon(String PathtoCall, SellerParams Seller, IAMUser UserIAM, Method TypeMethod, List<QueryParamas> ParamsGET = null, dynamic JsonBody = null)
        {
            AssumeRoleResponse assumeRoleResponse = null;
            

            assumeRoleResponse = GetAssumeRoleTokenDetail(UserIAM);

            if (assumeRoleResponse.SourceIdentity != null && assumeRoleResponse.SourceIdentity.Contains("ErrorApIAtelier"))
            {
                resp = assumeRoleResponse.SourceIdentity;
                return;
            }

            RestClient restClient = new RestClient(url_host);
            IRestRequest restRequest = new RestRequest(PathtoCall, TypeMethod);

            if (ParamsGET != null)
            {
                foreach (QueryParamas Param in ParamsGET)
                {
                    restRequest.AddParameter(Param.Item, Param.Value, ParameterType.QueryString);
                }
            }

            if(JsonBody != null)
            {
                restRequest.AddJsonBody(JsonBody);
            }
          
            AWSAuthenticationCredentials AWSCredentials = new AWSAuthenticationCredentials();
            AWSCredentials.AccessKeyId = assumeRoleResponse.Credentials.AccessKeyId;
            AWSCredentials.SecretKey = assumeRoleResponse.Credentials.SecretAccessKey;
            AWSCredentials.Region = Amazon.RegionEndpoint.EUWest1.SystemName;

            LWAAuthorizationCredentials LWACredentials = new LWAAuthorizationCredentials();
            LWACredentials.ClientId = Seller.client_id;
            LWACredentials.ClientSecret = Seller.client_secret;
            LWACredentials.Endpoint = new Uri(url_token);
            LWACredentials.RefreshToken = Seller.refresh_token;

            restRequest = new LWAAuthorizationSigner(LWACredentials).Sign(restRequest);

            if (restRequest.Resource != null && restRequest.Resource.Contains("ErrorApIAtelier"))
            {
                resp = restRequest.Resource;
                return;
            }

            restRequest.AddHeader("x-amz-security-token", assumeRoleResponse.Credentials.SessionToken);

            restRequest = new AWSSigV4Signer(AWSCredentials).Sign(restRequest, restClient.BaseUrl.Host);

            IRestResponse response = restClient.Execute(restRequest);

            resp = response.Content;
        }
        public class SellerParams
        {
            public string refresh_token { get; set; }
            public string client_id { get; set; }
            public string client_secret { get; set; }
        }

        public class QueryParamas
        {
            public string Item { get; set; }
            public string Value { get; set; }
        }

        public class IAMUser
        {
            public string AWSid { get; set; }
            public string AWSKeySecret { get; set; }
            public string AWSRole { get; set; }
        }
        private AssumeRoleResponse GetAssumeRoleTokenDetail(IAMUser User)
        {
            var accessKey = User.AWSid;
            var secretKey = User.AWSKeySecret;

            var assumeRoleRequest = new AssumeRoleRequest()
            {
                DurationSeconds = 3600,
                RoleArn = User.AWSRole,
                RoleSessionName = DateTime.Now.Ticks.ToString()
            };

            try
            {
      
            var credentials = new BasicAWSCredentials(accessKey, secretKey);

             var client = new AmazonSecurityTokenServiceClient(credentials, Amazon.RegionEndpoint.EUWest1);


            return client.AssumeRole(assumeRoleRequest);

            }
            catch (Exception e)
             {
                var assume = new AssumeRoleResponse();
                assume.SourceIdentity = "ErrorApIAtelier " + e.Message;
                return assume;
             }
            
        }
    }
}
