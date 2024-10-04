using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter the site link ");
        string siteLink = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(siteLink))
        {
            Console.WriteLine("Invalid site link.");
            return;
        }


        string baseUrl;
        if (siteLink.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            siteLink.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            baseUrl = siteLink;
        }
        else
        {
            baseUrl = "http://" + siteLink; 
        }

        string outputFilePath = "Successful.txt";

        try
        {
            string[] paths = File.ReadAllLines("paths.txt");
            using (HttpClient client = new HttpClient())
            using (StreamWriter writer = new StreamWriter(outputFilePath, false))
            {
                List<Task> tasks = new List<Task>();

                foreach (string path in paths)
                {
                    string url = baseUrl + path;
                    tasks.Add(CheckUrlAsync(client, writer, url));
                }

                await Task.WhenAll(tasks);
            }

            Console.WriteLine($"All successful paths have been saved to {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static async Task CheckUrlAsync(HttpClient client, StreamWriter writer, string url)
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            int statusCode = (int)response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                await writer.WriteLineAsync(url);
                await writer.FlushAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{url} - Status Code: {statusCode} (Mastered)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{url} - Status Code: {statusCode} (Not Mastered)");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{url} - Error: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
        }
    }
}
