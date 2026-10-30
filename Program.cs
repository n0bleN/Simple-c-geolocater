using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace GeolocatorApp
{
    public class IpData
    {
        public string ip { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string loc { get; set; }
        public string postal { get; set; }
        public string timezone { get; set; }
        public string error { get; set; }
    }

    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly List<string> history = new();

        static async Task Main()
        {
            Console.Title = "GEOLOCATER";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            client.DefaultRequestHeaders.UserAgent.ParseAdd("GeolocatorConsoleApp/1.0");

            PlayStartupAnimation();
            ShowTitle();

            while (true)
            {
                Console.Write("\nEnter IP | history | exit: ");
                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (input.Equals("history", StringComparison.OrdinalIgnoreCase))
                {
                    ShowHistory();
                    continue;
                }

                await LookupIp(input);

                Console.WriteLine("\nPress ENTER to lookup another IP...");
                Console.ReadLine();
                Console.Clear();
                ShowTitle();
            }
        }

        static async Task LookupIp(string ip)
        {
            try
            {
                string url = $"https://ipinfo.io/{ip}/json";
                string json = await client.GetStringAsync(url);
                IpData data = JsonConvert.DeserializeObject<IpData>(json);

                if (!string.IsNullOrEmpty(data.error))
                {
                    Console.WriteLine("API Error: " + data.error);
                    return;
                }

                history.Add(ip);

                Console.WriteLine("\n--- IP INFORMATION ---");
                Console.WriteLine($"IP Address : {data.ip ?? ip}");
                Console.WriteLine($"Country    : {data.country ?? "N/A"}");
                Console.WriteLine($"Region     : {data.region ?? "N/A"}");
                Console.WriteLine($"City       : {data.city ?? "N/A"}");
                Console.WriteLine($"Postal     : {data.postal ?? "N/A"}");
                Console.WriteLine($"Timezone   : {data.timezone ?? "N/A"}");
                

                if (!string.IsNullOrEmpty(data.loc))
                {
                    string[] coords = data.loc.Split(',');
                    Console.WriteLine($"Coordinates: {data.loc}");
                    Console.WriteLine($"Google Maps: https://www.google.com/maps/@{coords[0]},{coords[1]},12z");
                    Console.WriteLine("(Left ctrl + Click to open the Google Maps link in your browser).");
                }
                else
                {
                    Console.WriteLine("Coordinates: N/A (API limited)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lookup failed: " + ex.Message);
            }
        }

        static void ShowHistory()
        {
            Console.WriteLine("\n--- LOOKUP HISTORY ---");
            if (history.Count == 0)
            {
                Console.WriteLine("No history available.");
                return;
            }

            for (int i = 0; i < history.Count; i++)
                Console.WriteLine($"{i + 1}. {history[i]}");
        }

        static void ShowTitle()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=================================");
            Console.WriteLine("           GEOLOCATER             ");
            Console.WriteLine("=================================");
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void PlayStartupAnimation()
        {
            string[] frames =
            {
                "G","GE","GEO","GEOL","GEOLO","GEOLOC",
                "GEOLOCA","GEOLOCAT","GEOLOCATE","GEOLOCATER"
            };

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var f in frames)
            {
                Console.Clear();
                Console.WriteLine("\n\n\t\t" + f);
                Thread.Sleep(120);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
        }
    }
}
