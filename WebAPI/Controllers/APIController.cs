using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using WebAPI.CustomClasses;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : Controller
    {
        [HttpGet]
        public string Get()
        {

            //initialize database
            //create db handler
            CC_Database.DatabaseHandler DB_Handler = new CC_Database.DatabaseHandler();

            //path and name of db
            string databaseDirectory = DB_Handler.GetDatabaseDirectory();
            string databaseName = "MyDB";

            //create db if it doesnt exist
            DB_Handler.CreateDatabase(databaseDirectory, databaseName);

            //get all data necessary from the url
            HttpClient client = new HttpClient();
            var url = "https://restcountries.com/v3.1/all";

            var result = client.GetAsync(url).Result;
            var jObject = JsonDocument.Parse(result.Content.ReadAsStringAsync().Result);

            //create json handler 
            CC_Json.JsonHandler JS_Handler = new CC_Json.JsonHandler();

            string return_string = "";

            // Establish Connection with SQL SERVER
            SqlConnection conn = new SqlConnection("Server=localhost;Integrated security=SSPI;TrustServerCertificate=True");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "FAILED SQL SERVER CONNECTION";
            }
            finally
            {
                //if connection established begin json parsing and db insertion   
                //for each country
                int current_id = 0;
                foreach (var item in jObject.RootElement.EnumerateArray())
                {

                    //parse json for the properties in need
                    string country_commonName = item.GetProperty("name").GetProperty("common").ToString();
                    string country_capitals = JS_Handler.GetListedItems(item, "capital");
                    string country_borders = JS_Handler.GetListedItems(item, "borders"); ;

                    //add to the string
                    return_string += current_id+" Country: " + country_commonName + " / " +
                        "Capital(s): " + country_capitals + " / " +
                        "Bordering Countries: " + country_borders + "\n";

                    //insert current country's properties
                    DB_Handler.InsertRow(databaseName,cmd, current_id, country_commonName, country_capitals, country_borders);
                    current_id++;
                }
                //close the SQL SERVER Connection
                if (conn.State == ConnectionState.Closed)
                    conn.Close();
            }
            return return_string;
        }

        // POST api/<TestController>
        [HttpPost]
        public IActionResult Post([FromBody] CC_Requests.RequestObj BodyObj)
        {
            if (!BodyObj.ReqeustArrayObj.Any() || BodyObj.ReqeustArrayObj.Count() == 1)
                return BadRequest("ERROR:Invalid Body");

            CC_Requests.RequestHandler RQ_Handler = new CC_Requests.RequestHandler();
            return Ok(RQ_Handler.GetMax_AtPosition(BodyObj, 1));
        }

        //public string ClearDB()
        //{
        //    return "None";
        //}
    }
}
