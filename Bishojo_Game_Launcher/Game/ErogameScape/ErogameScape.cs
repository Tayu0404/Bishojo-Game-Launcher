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
        static ErogameScape() {
            client = new HttpClient();
        }

        private static HttpClient client;

        public enum Mode {
            URL = 0,
            ID = 1,
        }

        public class Detaile {
            private string title, brand, sellday, mainImage, erogame;
            private List<string> illustrators, scenarios, composers, voices, singers, sampleCGs;

            public string Title {
                get { return title; }
                set { title = value; }
            }

            public string Brand {
                get { return brand; }
                set { brand = value; }
            }

            public string Sellday {
                get { return sellday; }
                set { sellday = value; }
            }

            public string MainImage {
                get { return mainImage; }
                set { mainImage = value; }
            }

            public string Erogame {
                get { return erogame; }
                set { erogame = value; }
            }

            public List<string> Illustrators {
                get { return illustrators; }
                set { illustrators = value; }
            }

            public List<string> Scenarios {
                get { return scenarios; }
                set { scenarios = value; }
            }

            public List<string> Composers {
                get { return composers; }
                set { composers = value; }
            }

            public List<string> Voices {
                get { return voices; }
                set { voices = value; }
            }

            public List<string> Singers {
                get { return singers; }
                set { singers = value; }
            }

            public List<string> SampleCGs {
                get { return sampleCGs; }
                set { sampleCGs = value; }
            }
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
            var url = $"https://erogamescape.dyndns.org/~ap2/ero/toukei_kaiseki/kensaku.php?category=game&word_category=name&word={gameTitle}&mode=normal";
            var source = await getAsync(url);

            var doc = default(IHtmlDocument);
            var parser = new HtmlParser();
            doc = await parser.ParseDocumentAsync(source);

            var detaileList = new List<Dictionary<string, string>>();
            var detaileElements = doc.QuerySelectorAll("#result > table > tbody > tr");
            var head = true;
            foreach (var detaileElemet in detaileElements) {
                string title, brand, sellday, detaileurl;
                Dictionary<string, string> detaile = new Dictionary<string, string>();
                if (head) {
                    head = false;
                    continue;
                }
                var data = detaileElemet.QuerySelectorAll("a");
                Console.WriteLine(data.Length);
                switch (data.Length) {
                    case 2:
                        title = data[0].TextContent;
                        brand = data[1].TextContent;
                        sellday = detaileElemet.QuerySelectorAll("td")[2].TextContent;
                        detaileurl = data[0].GetAttribute("href");

                        detaile = new Dictionary<string, string>() {
                        { "Title", title },
                        { "Brand", brand },
                        { "Sellday", sellday },
                        { "Detaileurl", detaileurl },
                    };
                        break;

                    case 3:
                        title = data[0].TextContent;
                        brand = data[2].TextContent;
                        sellday = detaileElemet.QuerySelectorAll("td")[2].TextContent;
                        detaileurl = data[0].GetAttribute("href");

                        detaile = new Dictionary<string, string>() {
                        { "Title", title },
                        { "Brand", brand },
                        { "Sellday", sellday },
                        { "Detaileurl", detaileurl },
                    };
                        break;

                    case (4):
                        title = data[0].TextContent;
                        brand = data[3].TextContent;
                        sellday = detaileElemet.QuerySelectorAll("td")[2].TextContent;
                        detaileurl = data[0].GetAttribute("href");

                        detaile = new Dictionary<string, string>() {
                        { "Title", title },
                        { "Brand", brand },
                        { "Sellday", sellday },
                        { "Detaileurl", detaileurl },
                    };
                        break;
                }
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

            var detaile = new Detaile();

            detaile.Title = doc.QuerySelector("#game_title > a").TextContent;
            detaile.Brand = doc.QuerySelector("#brand > td > a").TextContent;
            detaile.Sellday = doc.QuerySelector("#sellday > td > a").TextContent;
            detaile.MainImage = doc.QuerySelector("#main_image > a > img").GetAttribute("src");
            detaile.Erogame = doc.QuerySelector("#erogame > td").TextContent;

            var illustratorElements = doc.QuerySelectorAll("#genga > td > a");
            var illustrators = new List<string>();
            foreach (var illustratorElement in illustratorElements) {
                var illustrator = illustratorElement.TextContent;
                illustrators.Add(illustrator);
            }
            detaile.Illustrators = illustrators;

            var scenarioElements = doc.QuerySelectorAll("#shinario > td > a");
            var scenarios = new List<string>();
            foreach (var scenarioElement in scenarioElements) {
                var scenario = scenarioElement.TextContent;
                scenarios.Add(scenario);
            }
            detaile.Scenarios = scenarios;

            var composerElements = doc.QuerySelectorAll("#ongaku > td > a");
            var composers = new List<string>();
            foreach (var composeElement in composerElements) {
                var compose = composeElement.TextContent;
                composers.Add(compose);
            }
            detaile.Composers = composers;

            var voiceElements = doc.QuerySelectorAll("#seiyu > td > a");
            var voices = new List<string>();
            foreach (var voiceElement in voiceElements) {
                var voice = voiceElement.TextContent;
                voices.Add(voice);
            }
            detaile.Voices = voices;

            var singerElements = doc.QuerySelectorAll("#kasyu > td > a");
            var singers = new List<string>();
            foreach (var singerElement in singerElements) {
                var singer = singerElement.TextContent;
                singers.Add(singer);
            }
            detaile.Singers = singers;

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
            detaile.SampleCGs = sampleCGs;

            return detaile;
        }
    }
}