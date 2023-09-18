using System.Text.Json;

namespace WebAPI.CustomClasses
{
    public class CC_Requests
    {
        //the main class of the requests
        public class RequestObj
        {
            public IEnumerable<int> ReqeustArrayObj { get; set; } = default!;
        }

        public class RequestHandler
        {
            public void PrintAll(RequestObj obj)
            {
                if (obj.ReqeustArrayObj.Any())
                    foreach (var item in obj.ReqeustArrayObj)
                        Console.WriteLine(item);
            }

            //print the position-largest number from the RequestArrayObj
            public int GetMax_AtPosition(RequestObj obj, int position)
            {
                var answer = obj.ReqeustArrayObj.OrderByDescending(x => x).Skip(position).FirstOrDefault();
                return answer;
            }

            public JsonDocument GetJsonBody_FromURL(string url)
            {
                try
                {
                    //Open new client and get data
                    HttpClient client = new HttpClient();

                    var result = client.GetAsync(url).Result;
                    JsonDocument jObject = JsonDocument.Parse(result.Content.ReadAsStringAsync().Result);
                    return jObject;
                }
                catch (Exception ex) { 
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }
    }
}
