#r "System.Configuration"
#r "System.Data"
#r "Newtonsoft.Json"

using System.Net;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
//config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

public class Provenance
{
    public string TagUID { get; set; }
    public string Sourcing_Name { get; set; }
    public string Sourcing_Location { get; set; }
    public string Sourcing_DateTime { get; set; }
    public string Sourcing_ImageURL { get; set; }
    public string Manufacturer_Name { get; set; }
    public string Manufacturer_Location { get; set; }
    public string Manufacturer_DateTime { get; set; }
    public string Manufacturer_ImageURL { get; set; }
    public string Bulk_Name { get; set; }
    public string Bulk_Location { get; set; }
    public string Bulk_DateTime { get; set; }
    public string Bulk_ImageURL { get; set; }
    public string Product_Name { get; set; }
    public string Product_Location { get; set; }
    public string Product_DateTime { get; set; }
    public string Product_ImageURL { get; set; }
    public string Distributor_Name { get; set; }
    public string Distributor_Location { get; set; }
    public string Distributor_DateTime { get; set; }
    public string Distributor_ImageURL { get; set; }
    public string Retailer_Name { get; set; }
    public string Retailer_Location { get; set; }
    public string Retailer_DateTime { get; set; }
    public string Retailer_ImageURL { get; set; }
}

public class apiResponse
{
    public Provenance[] BottleDetails { get; set;}
    public byte Status { get; set; }
}

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "nfcID", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();
    var responseResult = new apiResponse();
  
    //setup database connections

    try {
        var str = ConfigurationManager.ConnectionStrings["connectedBottlesDB"].ConnectionString;
        
        using (SqlConnection conn = new SqlConnection(str)) 
        {
            conn.Open();

            var text = "SELECT TagId, Sourcing_Name ,Sourcing_Location, Sourcing_DateTime, Sourcing_ImageURL, Manufacturer_Name, Manufacturer_Location, Manufacturer_DateTime, Manufacturer_ImageURL, Bulk_Name, Bulk_Location, Bulk_DateTime, Bulk_ImageURL, Product_Name, Product_Location, Product_DateTime, Product_ImageURL, Distributor_Name, Distributor_Location, Distributor_DateTime, Distributor_ImageURL, Retailer_Name, Retailer_Location, Retailer_DateTime, Retailer_ImageURL, Consumer_Name, Consumer_Location, Consumer_DateTime, Consumer_ImageURL " +
  "FROM ProductProvenanceInfo where TagID = " + name; // +" FOR JSON PATH";
            //log.Info(text);


            using (SqlCommand cmd = new SqlCommand(text, conn))
            {
                var list = new List<Provenance>();
                var reader = cmd.ExecuteReader();
                

                if(!reader.HasRows) 
                {
                    //responseResult.BottleDetails = [];
                    responseResult.Status = 0;

                }
                else
                {   
                   

                    while (reader.Read())
                    {

                      list.Add(new Provenance { 
                          TagUID = reader.GetString(0), 
                          Sourcing_Name = reader.GetString(1),
                          Sourcing_Location = reader.GetString(2),
                          Sourcing_DateTime = reader.GetDateTime(3).ToString(),
                          Sourcing_ImageURL = reader.GetString(4),
                          Manufacturer_Name = reader.GetString(5),
                          Manufacturer_Location = reader.GetString(6),
                          Manufacturer_DateTime = reader.GetDateTime(7).ToString(),
                          Manufacturer_ImageURL  = reader.GetString(8),
                          Bulk_Name   = reader.GetString(9),
                          Bulk_Location  = reader.GetString(10),
                          Bulk_DateTime  = reader.GetDateTime(11).ToString(),
                          Bulk_ImageURL   = reader.GetString(12),
                          Product_Name  = reader.GetString(13),
                          Product_Location = reader.GetString(14),
                          Product_DateTime  = reader.GetDateTime(15).ToString(),
                          Product_ImageURL = reader.GetString(16),
                          //Distributor_Name = reader.GetString(17),
                         // Distributor_Location = reader.GetString(18),
                          //Distributor_DateTime = reader.GetDateTime(19).ToString(),
                          //Distributor_ImageURL = reader.GetString(20),
                          Retailer_Name = reader.GetString(21),
                          Retailer_Location = reader.GetString(22),
                          Retailer_DateTime = reader.GetDateTime(23).ToString(),
                          Retailer_ImageURL = reader.GetString(24)

                          });
                      
                    }

                     responseResult.BottleDetails = list.ToArray();
                    responseResult.Status = 1;

                }

            }

        }

    } catch (Exception ex) {
     // log.Error($"C# Http trigger function exception: {ex.Message}");
        return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.BadRequest };
    }
    
  
    var jsonToReturn = JsonConvert.SerializeObject(responseResult);


    return new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
    };
}

