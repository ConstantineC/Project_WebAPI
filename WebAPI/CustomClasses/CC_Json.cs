﻿using System.Text.Json;

namespace WebAPI.CustomClasses
{
    public class CC_Json
    {
        public class JsonHandler
        {
            public string GetListedItems(JsonElement currentCountry, string property_name)
            {
                string listed_items = "";
                JsonElement property;
                if (currentCountry.TryGetProperty(property_name, out property))
                {
                    foreach (var item in property.EnumerateArray())
                        listed_items += item + ",";
                    listed_items = listed_items.Substring(0, listed_items.Length - 1);
                }
                return listed_items;
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }
    }
}
