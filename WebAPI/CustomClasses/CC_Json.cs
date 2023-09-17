using System.Text.Json;

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
                        listed_items += item + " ";

                }
                return listed_items;
            }
        }
    }
}
