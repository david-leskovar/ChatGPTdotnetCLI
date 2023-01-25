using System.Text;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace ChatGPT
{
    public class Program
    {
        public static string Guess(string raw)
        {
            Console.WriteLine("---> GPT-3 API Returned Text:");

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(raw);


           

            Console.ResetColor();

            TextCopy.ClipboardService.SetText(raw);


            return raw;
        }

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Enter your API key");
            string apikey = Console.ReadLine();

            

            while (true)
            {

                Console.WriteLine("Ask a question");
                string question  = Console.ReadLine();


                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization",
                    $"Bearer {apikey}");

                var content = new StringContent(
                    "{\"model\": \"text-davinci-001\", \"prompt\": \"" + question +
                    "\",\"temperature\": 1,\"max_tokens\": 100}",
                    Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);
                string responseString = await response.Content.ReadAsStringAsync();

               


                if (!response.IsSuccessStatusCode)
                {
                    ResponseError? res = JsonSerializer.Deserialize<ResponseError>(responseString);


                    string guess = Guess(res.error.message);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"---> My guess at the command prompt is:  {guess}");
                    Console.ResetColor();
                    continue;
                }

                try
                {
                    var data = JsonConvert.DeserializeObject<dynamic>(responseString);



                    Guess(Convert.ToString(data!.choices[0].text));

                    
                    Console.ResetColor();
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Ka dajle");
                    Console.WriteLine(ex.Message);
                }



            }



            
        }


        public class ResponseError
        {
            public Error error { get; set; }
        }

        public class Error
        {
            public string? message { get; set; }
            public string? type { get; set; }
            public string? param { get; set; }
            public string? code { get; set; }
        }
    }
}