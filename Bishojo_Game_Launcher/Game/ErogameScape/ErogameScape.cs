using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using MihaZupan;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bishojo_Game_Launcher.Game.ErogameScape {
    class ErogameScape {
        public ErogameScape() {
            client = new HttpClient();
        }

        private static HttpClient client;

        public enum Mode {
            URL = 0,
            ID = 1,
        }

        public class Detaile {
            public Detaile(
                string title,
                string brand,
                string sellday,
                string mainImage,
                string erogame,
                List<string> illustrators,
                List<string> scenarios,
                List<string> composers,
                List<string> voices,
                List<string> singers,
                List<string> sampleCGs
            ) {
                this.Title = title;
                this.Brand = brand;
                this.Sellday = sellday;
                this.MainImage = mainImage;
                this.Erogame = erogame;
                this.Illustrators = illustrators;
                this.Scenarios = scenarios;
                this.Composers = composers;
                this.Voices = voices;
                this.Singers = singers;
                this.SampleCGs = sampleCGs;
			}

            public string Title { get; private set; }

            public string Brand { get; private set; }

            public string Sellday { get; private set; }

            public string MainImage { get; private set; }

            public string Erogame { get; private set; }

            public List<string> Illustrators { get; private set; }

            public List<string> Scenarios { get; private set; }

            public List<string> Composers { get; private set; }

            public List<string> Voices { get; private set; }

            public List<string> Singers { get; private set; }

            public List<string> SampleCGs { get; private set; }
        }

        public void UseHTTPProxy(string address, int port) {
            var handler = new HttpClientHandler();
            handler.Proxy = new WebProxy($"{address}:{port}");
            handler.UseProxy = true;
            client = new HttpClient(handler);
        }

        public void UseHTTPProxy(string address, int port, string account, string password) {
            var handler = new HttpClientHandler();
            handler.Proxy = new WebProxy($"{address}:{port}");
            handler.Proxy.Credentials = new NetworkCredential(account, password);
            handler.UseProxy = true;
            client = new HttpClient(handler);
        }

        public void UseSocks5Proxy(string address, int port) {
            var handler = new HttpClientHandler();
            var proxy = new HttpToSocks5Proxy(address, port);
            handler = new HttpClientHandler();
            handler.Proxy = proxy;
            handler.UseProxy = true;
            client = new HttpClient(handler);
        }

        public void UseSocks5Proxy(string address, int port, string account, string password) {
            var handler = new HttpClientHandler();
            var proxy = new HttpToSocks5Proxy(address, port, account, password);
            handler = new HttpClientHandler();
            handler.Proxy = proxy;
            handler.UseProxy = true;
            client = new HttpClient(handler);
        }

        private async Task<string> getAsync(string url) {
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<Dictionary<string, string>>> SearchGame(string gameTitle) {
            Console.WriteLine("SearchGame");
            var url = $"https://erogamescape.dyndns.org/~ap2/ero/toukei_kaiseki/kensaku.php?category=game&word_category=name&word={gameTitle}&mode=normal";
            var source = await getAsync(url);

            var doc = default(IHtmlDocument);
            var parser = new HtmlParser();
            doc = await parser.ParseDocumentAsync(source);

            var detaileList = new List<Dictionary<string, string>>();
            var detaileElements = doc.QuerySelectorAll("#result > table > tbody > tr");
            var head = true;
            foreach (var detaileElemet in detaileElements) {
                Dictionary<string, string> detaile = new Dictionary<string, string>();
                if (head) {
                    head = false;
                    continue;
                }
                var data = detaileElemet.QuerySelectorAll("td");
                detaile = new Dictionary<string, string>() {
                        { "Title", data[0].TextContent.Replace("OHP", "") },
                        { "Brand", data[1].TextContent },
                        { "Sellday", data[2].TextContent },
                        { "Detaileurl", data[0].QuerySelector("a").GetAttribute("href") },
                };
                detaileList.Add(detaile);
                Console.WriteLine(detaile["Title"]);
                Console.WriteLine(detaile["Brand"]);
                Console.WriteLine(detaile["Sellday"]);
                Console.WriteLine(detaile["Detaileurl"]);
            }
            Console.WriteLine(detaileList);
            return detaileList;
        }

        public async Task<Detaile> GetGameDetails(string detaileURL, Mode mode = Mode.URL) {
            var url = detaileURL;

            if (mode == Mode.ID) {
                url = $"https://erogamescape.dyndns.org/~ap2/ero/toukei_kaiseki/{detaileURL}";
            }

            var source = await getAsync(url);
            var doc = default(IHtmlDocument);
            var parser = new HtmlParser();
            doc = await parser.ParseDocumentAsync(source);

            var title = doc.QuerySelector("#game_title > a").TextContent;
            var brand = doc.QuerySelector("#brand > td > a").TextContent;
            var sellday = doc.QuerySelector("#sellday > td > a").TextContent;
            var mainImage = doc.QuerySelector("#main_image > a > img").GetAttribute("src");
            var erogame = doc.QuerySelector("#erogame > td").TextContent;

            var illustratorElements = doc.QuerySelectorAll("#genga > td > a");
            var illustrators = new List<string>();
            foreach (var illustratorElement in illustratorElements) {
                var illustrator = illustratorElement.TextContent;
                illustrators.Add(illustrator);
            }

            var scenarioElements = doc.QuerySelectorAll("#shinario > td > a");
            var scenarios = new List<string>();
            foreach (var scenarioElement in scenarioElements) {
                var scenario = scenarioElement.TextContent;
                scenarios.Add(scenario);
            }

            var composerElements = doc.QuerySelectorAll("#ongaku > td > a");
            var composers = new List<string>();
            foreach (var composeElement in composerElements) {
                var compose = composeElement.TextContent;
                composers.Add(compose);
            }

            var voiceElements = doc.QuerySelectorAll("#seiyu > td > a");
            var voices = new List<string>();
            foreach (var voiceElement in voiceElements) {
                var voice = voiceElement.TextContent;
                voices.Add(voice);
            }

            var singerElements = doc.QuerySelectorAll("#kasyu > td > a");
            var singers = new List<string>();
            foreach (var singerElement in singerElements) {
                var singer = singerElement.TextContent;
                singers.Add(singer);
            }

            var dmmSampleCGElements = doc.QuerySelectorAll("#dmm_sample_cg_main > a > img");
            var dlsiteSampleCGElements = doc.QuerySelectorAll("#dlsite_sample_cg_1_main > a > img");
            var sampleCGs = new List<string>();
            foreach (var sampleCGElement in dmmSampleCGElements) {
                var sampleCG = sampleCGElement.GetAttribute("src");
                sampleCGs.Add(sampleCG);
            }
            foreach (var sampleCGElement in dlsiteSampleCGElements) {
                var sampleCG = sampleCGElement.GetAttribute("src");
                sampleCGs.Add(sampleCG);
            }

            var detaile = new Detaile(
                title,
                brand,
                sellday,
                mainImage,
                erogame,
                illustrators,
                scenarios,
                composers,
                voices,
                singers,
                sampleCGs
            );

            return detaile;
        }
    }
}