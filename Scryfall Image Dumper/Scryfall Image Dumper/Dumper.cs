using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scryfall_Image_Dumper {
    class Dumper : IDisposable {

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private string URL { get; set; }
        private int SwampCount = 0;
        private int PlainsCount = 0;
        private int MountainCount = 0;
        private int IslandCount = 0;
        private int ForestCount = 0;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Dumper()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// A disposable object that is used to download all card images from a specified set of MTG cards.
        /// </summary>
        /// <param name="url">The URL of the scryfall.com set page.</param>
        public Dumper(string url) {
            URL = url;
        }

        /// <summary>
        /// Runs the dump.
        /// </summary>
        public void PerformDump() {
            string html = GetSiteHTML(URL);

            List<string> cardURLs = GetCardURLs(html);

            List<string> imageURLs = GetImageURLs(cardURLs);

            List<string> cardNames = GetCardNames(cardURLs);

            DumpAllImages(imageURLs, cardNames);
        }

        /// <summary>
        /// Downloads the HTML text from the given website.
        /// </summary>
        /// <param name="url">The URL of the scryfall website.</param>
        /// <returns>The html of the website.</returns>
        private string GetSiteHTML(string url) {
            string html = "";
            try {
                using (WebClient client = new WebClient()) {
                    Console.WriteLine("HTML string loaded successfully.");
                    html = client.DownloadString($"https://{url}");
                }
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
            return html;
        }

        /// <summary>
        /// Gets every card link using regular expressions.
        /// </summary>
        /// <param name="html">The HTMl of the website.</param>
        /// <returns>A list of card URLs.</returns>
        private List<string> GetCardURLs(string html) {
            Regex rx = new Regex("<a class=\"card-grid-item-card\" href=\"(.*)\">");
            MatchCollection matches = rx.Matches(html);
            List<string> cardURLs = new List<string>();
            int count = 1;
            foreach (Match match in matches) {
                Console.WriteLine($"({count}/{matches.Count}) {match.Groups[1].Value}");
                cardURLs.Add(match.Groups[1].Value);
                count++;
            }
            return cardURLs;
        }

        /// <summary>
        /// Based on card URLs givem, it uses regular expressions to find the name of the card.
        /// </summary>
        /// <param name="cardURLs">The list of card URLs.</param>
        /// <returns>A list of names of each card based on their URL.</returns>
        private List<string> GetCardNames(List<string> cardURLs) {
            Regex rx = new Regex("card/.*/.*/(.*)");
            List<string> cardNames = new List<string>();
            foreach (string url in cardURLs) {
                string temp = rx.Match(url).Groups[1].Value;
                switch(temp) {
                    case "swamp":
                        SwampCount++;
                        temp += $"-{SwampCount}";
                        break;
                    case "plains":
                        PlainsCount++;
                        temp += $"-{PlainsCount}";
                        break;
                    case "island":
                        IslandCount++;
                        temp += $"-{IslandCount}";
                        break;
                    case "mountain":
                        MountainCount++;
                        temp += $"-{MountainCount}";
                        break;
                    case "forest":
                        ForestCount++;
                        temp += $"-{ForestCount}";
                        break;
                }
                cardNames.Add(temp);
            }
            return cardNames;
        }

        /// <summary>
        /// Gets the image URLs for each cardURL given.
        /// </summary>
        /// <param name="cardURLs">The list of card URLs.</param>
        /// <returns>A list containing all card image URLs.</returns>
        private List<string> GetImageURLs(List<string> cardURLs) {
            Regex rx = new Regex("class=\"card .* border-black\".*src=\"(.*)\" />");
            List<string> imageURLs = new List<string>();
            try {
                using (WebClient client = new WebClient()) {
                    int count = 1;
                    foreach (string url in cardURLs) {
                        string html = client.DownloadString(url);
                        string result = rx.Match(html).Groups[1].Value;
                        Console.WriteLine($"({count}/{cardURLs.Count}) {result}");
                        imageURLs.Add(result);
                        count++;
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
            return imageURLs;
        }

        /// <summary>
        /// Using all of the Card Names and Card Image URLs, this functions creates a folder within the projects' directory and dumps the images
        /// there as jpegs.
        /// </summary>
        /// <param name="imageURLs">A list containing the Image URLs.</param>
        /// <param name="cardNames">A list containing the Card Names.</param>
        private void DumpAllImages(List<string> imageURLs, List<string> cardNames) {
            try {
                using (WebClient client = new WebClient()) {
                    Directory.CreateDirectory("image-dump");
                    int count = 1;
                    for (int i = 0; i < imageURLs.Count; i++) {
                        client.DownloadFile(new Uri(imageURLs[i]), $"image-dump/{cardNames[i]}.jpg");
                        Console.WriteLine($"({count}/{imageURLs.Count}) {Directory.GetCurrentDirectory()}/image-dump/{cardNames[i]}.jpg");
                        count++;
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
