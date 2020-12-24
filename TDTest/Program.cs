using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace TDTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            var client = new HttpClient();

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Axr4eKl0AHE9nd4Yk4KI3HLhBczmCOsU40J+5878nLKOGeVdTRNz9QhYhwcocLkCV0J5uWgN/V2z4udpiMjS+WJustDAiVK/pOPAzB/Ef4jYEKO7Y5gZXTmGg+wme/L04NYAZqlWYo5eBG6UDh4/ZjUJD4GSU0qEfl03EXyoKAfMBStvYwZKr1amy7LnqJbcieyfgyDz8QSSpOnswskdExj/bUINY3z4JTSGsxfsMc4M8gSK76ZoirgeShVHPYF1G0Zow5w1/KIDW/KyxdqedMle1+uA5ijVp24hrPRC9gNEG8Eu6f5wl/MTTR8jBkNelzKsx/PUzw+na3qbszHXqgnuUsXSudEG9zvyjtA3cNa0qWJ02hvP9lkQYIKCgQ+zqUHgg5Qq+LfHMk2mHW6TVOG6wiUvidx6MXmhrthWtgtMyz2uT2XcDizQaabh0uC16Y5iQoAO8CGvtDk0QadhNAIaFPjJoa2vGiMaSkRL3J7NslgZYrEowb0EZkH/evydWnuc6R15V47meARANCi5Q+LfX3tVzGC+4hRwgR5LU2bIylhr2FxqgYAgMZ+n6EWPbVIkk100MQuG4LYrgoVi/JHHvlZY799xw7jnI64tvcATsp1HM1USmEdRc8C+eVgcSPSmYQFrMBQW0ER0lxfHquHlus4vabdZeRk1hn76ayHhSgJBr6nYgar3CjYYKH1mvGmjFlqx2uee9dYfYUAEx198jSfYwz93ONaOg1hC29scrZkk6OpjodbX8yvzhptFiXMYrPJEoHLqQh46Z+IaC68EWSuLKU0avNH4f6auhX6rzmpfpQyxZpRUMYAn8JHz41fxtAAkupNOJ+CPYTmXdJw8r6sa8yjxQC0Zlrs+D09QzjqSvrFxkCq0+LIAP/WMsvStQq7JOxkYLj/QpBGt5DVAiCYEjaTTLv4Q23w3lYkw2pXxCH1le4KzxgE7HsNay3xQ/ycGBk5wiGfjm43xVpB/l52Er4OtWy3ZrnDSd1oJnjNcgGuW0PlNNFJLKvy36VroBUgFK15LvXZ43ALLVZj+Uhiqvog9lnzXSQpRWKXjoIzfdCHVI0lflFFjego43b2nTAZh56Ee9pxMnJC2cyUUs4juAw656OdJDrkuH60fkl5q4fYzpzSKFzARGJ663Nusvok9InBTCI5vQMXbfqTnE5qvgijs212FD3x19z9sWBHDJACbC00B75E");
            var response = client.GetAsync("https://api.tdameritrade.com/v1/marketdata/chains?apikey=6ANM3TCMXETNKQLNHSCYJCLYNGKHUHLS%40AMER.OAUTHAP&symbol=AAPL&contractType=ALL&strikeCount=100");
            var responseString = response.Result.Content.ReadAsStringAsync();

            //var requestBody = new RequestBody();
            //requestBody.DateMonth = "01-MAY-18";
            //var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            //var req = new HttpRequestMessage(HttpMethod.Get, "https://api.tdameritrade.com/v1/marketdata/chains?apikey=6ANM3TCMXETNKQLNHSCYJCLYNGKHUHLS%40AMER.OAUTHAP&symbol=AAPL");
            //req.Headers.Add("Authorization", "Axr4eKl0AHE9nd4Yk4KI3HLhBczmCOsU40J+5878nLKOGeVdTRNz9QhYhwcocLkCV0J5uWgN/V2z4udpiMjS+WJustDAiVK/pOPAzB/Ef4jYEKO7Y5gZXTmGg+wme/L04NYAZqlWYo5eBG6UDh4/ZjUJD4GSU0qEfl03EXyoKAfMBStvYwZKr1amy7LnqJbcieyfgyDz8QSSpOnswskdExj/bUINY3z4JTSGsxfsMc4M8gSK76ZoirgeShVHPYF1G0Zow5w1/KIDW/KyxdqedMle1+uA5ijVp24hrPRC9gNEG8Eu6f5wl/MTTR8jBkNelzKsx/PUzw+na3qbszHXqgnuUsXSudEG9zvyjtA3cNa0qWJ02hvP9lkQYIKCgQ+zqUHgg5Qq+LfHMk2mHW6TVOG6wiUvidx6MXmhrthWtgtMyz2uT2XcDizQaabh0uC16Y5iQoAO8CGvtDk0QadhNAIaFPjJoa2vGiMaSkRL3J7NslgZYrEowb0EZkH/evydWnuc6R15V47meARANCi5Q+LfX3tVzGC+4hRwgR5LU2bIylhr2FxqgYAgMZ+n6EWPbVIkk100MQuG4LYrgoVi/JHHvlZY799xw7jnI64tvcATsp1HM1USmEdRc8C+eVgcSPSmYQFrMBQW0ER0lxfHquHlus4vabdZeRk1hn76ayHhSgJBr6nYgar3CjYYKH1mvGmjFlqx2uee9dYfYUAEx198jSfYwz93ONaOg1hC29scrZkk6OpjodbX8yvzhptFiXMYrPJEoHLqQh46Z+IaC68EWSuLKU0avNH4f6auhX6rzmpfpQyxZpRUMYAn8JHz41fxtAAkupNOJ+CPYTmXdJw8r6sa8yjxQC0Zlrs+D09QzjqSvrFxkCq0+LIAP/WMsvStQq7JOxkYLj/QpBGt5DVAiCYEjaTTLv4Q23w3lYkw2pXxCH1le4KzxgE7HsNay3xQ/ycGBk5wiGfjm43xVpB/l52Er4OtWy3ZrnDSd1oJnjNcgGuW0PlNNFJLKvy36VroBUgFK15LvXZ43ALLVZj+Uhiqvog9lnzXSQpRWKXjoIzfdCHVI0lflFFjego43b2nTAZh56Ee9pxMnJC2cyUUs4juAw656OdJDrkuH60fkl5q4fYzpzSKFzARGJ663Nusvok9InBTCI5vQMXbfqTnE5qvgijs212FD3x19z9sWBHDJACbC00B75E");
            //req.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //var response = client.SendAsync(req);
            //var responseString = response.Result.Content.ReadAsStringAsync().Result.Replace(" ", "");
            //var responseObject = JsonConvert.DeserializeObject<ResponseBody>(responseString);

            Console.ReadLine();
        }
    }

    public class RequestBody
    {
        [JsonProperty("dateMonth")]
        public string DateMonth { get; set; }
    }
}
