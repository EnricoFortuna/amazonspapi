"# amazonspapi" 

example use


var Seller = new ConnectToAmazon.SellerParams();
Seller.refresh_token = ;
Seller.client_id = "";
Seller.client_secret = "";

var User = new ConnectToAmazon.IAMUser();
User.AWSid = "";
User.AWSKeySecret = "";
User.AWSRole = "";

string urltocall  = "/reports/2021-06-30/reports";

var listMarket = new List<String>();
listMarket.Add("APJ6JRA9NG5V4");
  
var Jsonbody = new
{
    reportType = "GET_MERCHANT_LISTINGS_ALL_DATA",
    dataStartTime = "2019-12-10T20:11:24.000Z",
    marketplaceIds = listMarket
};

var AmazonCon = new ConnectToAmazon(urltocall, Seller, User, Method.POST, null, Jsonbody);
richTextBox1.Text = AmazonCon.resp;
