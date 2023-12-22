using EstdCoReportApp.Application.Contract;
using EstdCoReportApp.Application.Domain;
using EstdCoReportApp.Application.HttpClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using WordCloudSharp;
using System.Threading.Tasks;
using System.Drawing;

namespace EstdCoReportApp.Application
{
    public class WordCloudDataService : IWordCloudDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WordCloudDataService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> GenerateWordCloud(string url)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            string htmlContent = await httpClient.GetStringAsync(url);
            string textContent = ExtractTextFromHtml(htmlContent);
            // تبدیل html به متن
            //string textContent = htmlDocument.DocumentNode.InnerText;



            // شمردن کلمات
            Dictionary<string, int> wordCount = CountWords(textContent);

            var image = GenerateWordCloudImage(wordCount);
            var base64 = ConvertImageToBase64(image);

            Console.WriteLine($"Word Count: {wordCount.Count}");
            return base64;


            // بارگذاری محتوا
            //HtmlDocument htmlDocument = new HtmlDocument();
            //htmlDocument.LoadHtml(htmlContent);







        }
        private string ExtractTextFromHtml(string htmlContent)
        {
            // Use HtmlAgilityPack to parse HTML and extract text
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            // Concatenate all text nodes in the HTML document
            return string.Join(" ", htmlDocument.DocumentNode.DescendantsAndSelf()
                .Where(n => !n.HasChildNodes)
                .Select(n => n.InnerText.Trim()));
        }
        private Dictionary<string, int> CountWords(string text)
        {
            MatchCollection matches = Regex.Matches(text, @"\b\w+\b");

            // بررسی تعداد هر کلمه
            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            foreach (Match match in matches)
            {
                string word = match.Value.ToLower();
                if (wordCount.ContainsKey(word))
                {
                    wordCount[word]++;
                }
                else
                {
                    wordCount[word] = 1;
                }
            }

            return wordCount;
        }

        private Image GenerateWordCloudImage(Dictionary<string, int> wordCount)
        {
            // Create a WordCloud instance
            var wordCloud = new WordCloud(800, 600, fontname: "Arial");

            //// Add words and their frequencies to the WordCloud instance
            //foreach (var entry in wordCount)
            //{
            //    wordCloud.Draw(entry.Key, entry.Value);
            //}

            // Generate the word cloud image
            Image image = wordCloud.Draw(wordCount.Keys.ToList(), wordCount.Values.ToList());
            return image;
            // Save the word cloud image to a file
            image.Save("E:\\wordcloud.png");

            Console.WriteLine("Word cloud generated and saved as 'd:\\wordcloud.png'.");
        }
        private string ConvertImageToBase64(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert the image to a MemoryStream
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                // Convert the MemoryStream to a byte array
                byte[] imageBytes = ms.ToArray();

                // Convert the byte array to a Base64 string
                string base64Image = Convert.ToBase64String(imageBytes);

                return base64Image;
            }
        }

    }
}