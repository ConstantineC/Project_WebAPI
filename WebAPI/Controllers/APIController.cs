using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            CC_Database.DatabaseHandler DB_Handler = new CC_Database.DatabaseHandler();
            string databaseDirectory = DB_Handler.GetDatabaseDirectory();
            DB_Handler.CreateDatabase(databaseDirectory);

            HttpClient client = new HttpClient();
            var url = "https://restcountries.com/v3.1/all";

            var result = client.GetAsync(url).Result;
            var jObject = JsonDocument.Parse(result.Content.ReadAsStringAsync().Result);
            CC_Json.JsonHandler JS_Handler = new CC_Json.JsonHandler();

            string return_string = "";
            foreach (var item in jObject.RootElement.EnumerateArray())
            {
                string country_commonName = item.GetProperty("name").GetProperty("common").ToString();
                string country_capitals = JS_Handler.GetListedItems(item, "capital");
                string country_borders = JS_Handler.GetListedItems(item, "borders"); ;

                return_string += "Country: " + country_commonName + " / " +
                    "Capital(s): " + country_capitals + " / " +
                    "Bordering Countries: " + country_borders + "\n";

                DB_Handler.InsertRow(country_commonName);
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
    }
}
