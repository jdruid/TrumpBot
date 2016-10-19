using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TrumpBot
{
    public class Trump
    {
        public static async Task<PersonalizedMessage> GetPersonalizedMessage(string yourname)
        {
            if (string.IsNullOrWhiteSpace(yourname))
                return null;

            string url = $"https://api.whatdoestrumpthink.com/api/v1/quotes/personalized?q={yourname}";
           
            string json = await GetJson(url);

            PersonalizedMessage pm = null;
            try
            {
                pm = JsonConvert.DeserializeObject<PersonalizedMessage>(json);
            }
            catch (Exception e)
            {

            }

            return pm;
        }

        public static async Task<string> GetRandomQuote()
        {
            string url = "https://api.whatdoestrumpthink.com/api/v1/quotes/random";
            
            string json = await GetJson(url);

            RandomQuote quote = null;
            try
            {
                quote = JsonConvert.DeserializeObject<RandomQuote>(json);
            }
            catch (Exception e)
            {

            }

            return quote.message;
        }

        public static async Task<AllQuotes> GetAllQuotes()
        {
            string url = "https://api.whatdoestrumpthink.com/api/v1/quotes/";

            string json = await GetJson(url);

            AllQuotes m = null;
            try
            {
                m = JsonConvert.DeserializeObject<AllQuotes>(json);
            }
            catch (Exception e)
            {

            }

            return m;
        }

        private static async Task<string> GetJson(string endpoint)
        {
            string json = string.Empty;
            using (WebClient client = new WebClient())
            {
                json = await client.DownloadStringTaskAsync(endpoint).ConfigureAwait(false);
            }

            return json;
        }
        
    }

    public class RandomQuote
    {
        public string message { get; set; }      
    }

    public class PersonalizedMessage
    {
        public string nickname { get; set; }
        public string message { get; set; }
        
    }

    public class Messages
    {
        public List<string> personalized { get; set; }
        public List<string> non_personalized { get; set; }
    }

    public class AllQuotes
    {
        public Messages messages { get; set; }
    }
   
   

}
