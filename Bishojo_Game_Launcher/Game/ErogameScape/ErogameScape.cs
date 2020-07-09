using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using MihaZupan;
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

        private async Task<string> findGame(string gameName) {
            var url = $"https://erogamescape.dyndns.org/~ap2/ero/toukei_kaiseki/kensaku.php?category=game&word_category=name&word={gameName}&mode=normal";
            var source = await getAsync(url);

            var doc = default(IHtmlDocument);
            var parser = new HtmlParser();
            doc = await parser.ParseDocumentAsync(source);
            var detaileURL = doc.QuerySelector("a.tooltip").GetAttribute("href");
            return detaileURL;
        }

        public async Task GetGameDetails(string gameName) {
            var detaileURL = await findGame(gameName);
            var url = $"https://erogamescape.dyndns.org/~ap2/ero/toukei_kaiseki/{detaileURL}";
            var source = await getAsync(url);

            var doc = default(IHtmlDocument);
            var parser = new HtmlParser();
            doc = await parser.ParseDocumentAsync(source);

            this.Title = doc.QuerySelector("#game_title > a").TextContent;
            this.Brand = doc.QuerySelector("#brand > td > a").TextContent;
            this.Sellday = doc.QuerySelector("#sellday > td > a").TextContent;
            this.MainImage = doc.QuerySelector("#main_image > a > img").GetAttribute("src");
            this.Erogame = doc.QuerySelector("#erogame > td").TextContent;

            var illustratorElements = doc.QuerySelectorAll("#genga > td > a");
            var illustrators = new List<string>();
            foreach (var illustratorElement in illustratorElements) {
                var illustrator = illustratorElement.TextContent;
                illustrators.Add(illustrator);
            }
            this.Illustrators = illustrators;

            var scenarioElements = doc.QuerySelectorAll("#shinario > td > a");
            var scenarios = new List<string>();
            foreach (var scenarioElement in scenarioElements) {
                var scenario = scenarioElement.TextContent;
                scenarios.Add(scenario);
            }
            this.Scenarios = scenarios;

            var composerElements = doc.QuerySelectorAll("#ongaku > td > a");
            var composers = new List<string>();
            foreach (var composeElement in composerElements) {
                var compose = composeElement.TextContent;
                composers.Add(compose);
            }
            this.Composers = composers;

            var voiceElements = doc.QuerySelectorAll("#seiyu > td > a");
            var voices = new List<string>();
            foreach (var voiceElement in voiceElements) {
                var voice = voiceElement.TextContent;
                voices.Add(voice);
            }
            this.Voices = voices;

            var singerElements = doc.QuerySelectorAll("#kasyu > td > a");
            var singers = new List<string>();
            foreach (var singerElement in singerElements) {
                var singer = singerElement.TextContent;
                singers.Add(singer);
            }
            this.Singers = singers;

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
            this.SampleCGs = sampleCGs;
        }
    }
}
