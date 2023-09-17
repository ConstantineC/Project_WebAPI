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

        }
    }
}
