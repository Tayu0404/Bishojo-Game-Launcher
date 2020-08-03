using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using MihaZupan;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BishojoGameLauncher.Game {
    class ErogameScape {
        public ErogameScape() {
            client = new HttpClient();
        }

        private static HttpClient client;

        public enum GetDetaileMode {
            URL = 0,
            ID = 1,
        }

        public enum SearchGameMode {
            Title = 0,
            Brand = 1,
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

        public async Task<List<Game.SearchResult>> SearchGame(string searchWord, SearchGameMode mode = SearchGameMode.Title) {
            string url;
            if (mode == SearchGameMode.Brand) {
                url = $"https://erogamescape.dyndns.org/~ap2/ero/toukei_kaiseki/kensaku.php?category=brand&word_category=name&word={searchWord}&mode=normal";
            } else {
                url = $"https://erogamescape.dyndns.org/~ap2/ero/toukei_kaiseki/kensaku.php?category=game&word_category=name&word={searchWord}&mode=normal";
            }

            var source = await getAsync(url);

            var doc = default(IHtmlDocument);
            var parser = new HtmlParser();
            doc = await parser.ParseDocumentAsync(source);

            var detaileList = new List<Game.SearchResult>();
            var detaileElements = doc.QuerySelectorAll("#result > table > tbody > tr");
            var head = true;
            foreach (var detaileElemet in detaileElements) {
                if (head) {
                    head = false;
                    continue;
                }
                var data = detaileElemet.QuerySelectorAll("td");
                if (mode == SearchGameMode.Brand) {
                    var detaile = new Game.SearchResult(
                        data[0].TextContent.Replace("OHP", ""),
                        doc.QuerySelector("#result > h3 > a").TextContent,
                        data[1].TextContent,
                        data[0].QuerySelector("a").GetAttribute("href")
                    );
                    detaileList.Add(detaile);
                } else {
                    var detaile = new Game.SearchResult(
                        data[0].TextContent.Replace("OHP", ""),
                        data[1].TextContent,
                        data[2].TextContent,
                        data[0].QuerySelector("a").GetAttribute("href")
                    );
                    detaileList.Add(detaile);
                }
            }
            return detaileList;
        }

        public async Task<Detaile> GetGameDetails(string detaileURL, GetDetaileMode mode = GetDetaileMode.URL) {
            var url = detaileURL;

            if (mode == GetDetaileMode.ID) {
                url = $"https://erogamescape.dyndns.org/~ap2/ero/toukei_kaiseki/{detaileURL}";
            }

            var source = await getAsync(url);
            var doc = default(IHtmlDocument);
            var parser = new HtmlParser();
            doc = await parser.ParseDocumentAsync(source);

            var title = doc.QuerySelector("#game_title > a").TextContent;
            var web = doc.QuerySelector("#game_title > a").GetAttribute("href");
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

            var characterElements = doc.QuerySelectorAll("#seiyu > td > span");
            var characters = new List<string>();
            foreach (var characterElement in characterElements) {
                var character = characterElement.TextContent.Trim('(', ')');
                characters.Add(character);
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
                web,
                brand,
                sellday,
                mainImage,
                erogame,
                illustrators,
                scenarios,
                composers,
                voices,
                characters,
                singers,
                sampleCGs
            );

            return detaile;
        }
    }
}