using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VSOSamples.Console
{
    class Program
    {
        static async Task GetTeamProjects(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);

                HttpResponseMessage response = await client.GetAsync("/DefaultCollection/_apis/projects?api-version=1.0");
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine(result);
            }
        }

        static void Main(string[] args)
        {
            GetTeamProjects("https://rlbisbe.VisualStudio.com").Wait();
        }
    }
}
