using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Runtime.Caching;
using System.Text.Json;
using WebAPI.CustomClasses;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : Controller
    {
        private string _DatabaseName = "MyDB";
        private readonly string _CacheName ="CountriesCache";
        private readonly string _CacheKey ="CountriesCacheKey";
        private float _CacheExpirationAmount = 10;

        [HttpGet]
        public string Get()
        {
            //initialize cache
            CC_Cache.CacheHandler Cache_Handler = new CC_Cache.CacheHandler();
            ObjectCache cache = MemoryCache.Default;

            CC_Cache.CacheObject currentObj = cache.Get(_CacheKey) as CC_Cache.CacheObject;
            if (currentObj == null)
                Console.WriteLine("Empty cache");
            else
                return "Cache\n"+currentObj.content;

            //create db handler
            CC_Database.DatabaseHandler DB_Handler = new CC_Database.DatabaseHandler();

            //establish connection with server
            SqlConnection sqlConnection = new SqlConnection("Server=localhost;Integrated security=SSPI;TrustServerCertificate=True");
            SqlCommand sqlCommand = DB_Handler.EstablishConnection(sqlConnection);

            if (sqlCommand == null)
                return "FAILED TO CONNECT TO SQL SERVER";

            //if database exists
            if (DB_Handler.DatabaseExists(sqlCommand, _DatabaseName))
            {
                Console.WriteLine("Got data from DB");
                string result_string= DB_Handler.GetAllRows(sqlCommand, _DatabaseName);

                //close connection with the sql server
                DB_Handler.CloseConnection(sqlConnection);

                //add to cache
                Cache_Handler.AddContent(cache, _CacheKey, result_string, _CacheExpirationAmount);

                return "DB\n" + result_string;
            }

            //create json handler 
            CC_Json.JsonHandler JS_Handler = new CC_Json.JsonHandler();

            //create new database
            DB_Handler.CreateDatabase(_DatabaseName, sqlCommand);
            Console.WriteLine("Created new DB");

            //http call for the data
            string url = "https://restcountries.com/v3.1/all";
            JsonDocument jObject= JS_Handler.GetJsonBody_FromURL(url);
            if (jObject == null)
                return "ERROR on HTTP call";


            string return_string = "";
            int current_id = 0;
            //foreach country
            foreach (var item in jObject.RootElement.EnumerateArray())
            {
                //parse json for the properties in need
                string country_commonName = item.GetProperty("name").GetProperty("common").ToString();
                string country_capitals = JS_Handler.GetListedItems(item, "capital");
                string country_borders = JS_Handler.GetListedItems(item, "borders");

                if (country_capitals.IsNullOrEmpty()) country_capitals = "None";
                if (country_borders.IsNullOrEmpty()) country_borders = "None";

                //add to the string
                return_string += current_id+" Country: " + country_commonName + " / " +
                    "Capital(s): " + country_capitals + " / " +
                    "Bordering Countries: " + country_borders + "\n";

                //insert current country's properties
                DB_Handler.InsertRow(_DatabaseName, sqlCommand, current_id, country_commonName, country_capitals, country_borders);

                current_id++;
            }

            //close connection with the sql server
            DB_Handler.CloseConnection(sqlConnection);

            //add result to cache
            Cache_Handler.AddContent(cache, _CacheKey, return_string, _CacheExpirationAmount);
            return "HTTP Call\n" +return_string;
        }

        // POST api/<TestController>
        [HttpPost]
        public IActionResult Post([FromBody] CC_Requests.RequestObj BodyObj)
        {
            //if empty body or smaller than the position wanted
            if (!BodyObj.ReqeustArrayObj.Any() || BodyObj.ReqeustArrayObj.Count() == 1)
                return BadRequest("ERROR:Invalid Body");

            //return the largest number depending on position given
            CC_Requests.RequestHandler RQ_Handler = new CC_Requests.RequestHandler();
            return Ok(RQ_Handler.GetMax_AtPosition(BodyObj, 1));
        }


        //delete function used to clear database and connection with it 
        //used to test the functionality of cache-db-http call system
        [HttpDelete]
        public string Delete()
        {

            //create db handler
            CC_Database.DatabaseHandler DB_Handler = new CC_Database.DatabaseHandler();

            //establish connection with server
            SqlConnection sqlConnection = new SqlConnection("Server=localhost;Integrated security=SSPI;TrustServerCertificate=True");
            SqlCommand sqlCommand = DB_Handler.EstablishConnection(sqlConnection);

            string deletionResult;
            //if db doesnt exist
            if (DB_Handler.DatabaseExists(sqlCommand, _DatabaseName))
                deletionResult = DB_Handler.DeleteDatabase(sqlCommand, _DatabaseName);
            else
                deletionResult = "Database " + _DatabaseName + " doesn't exist ";

            DB_Handler.CloseConnection(sqlConnection);

            return deletionResult;
        }
    }
}
