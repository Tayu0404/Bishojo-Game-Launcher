using Bishojo_Game_Launcher.Property;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bishojo_Game_Launcher.Game {
	class Game {
        public class SearchResult {
            public SearchResult(
                string title,
                string brand,
                string sellday,
                string detaileirl
            ) {
                this.Title = title;
                this.Brand = brand;
                this.Sellday = sellday;
                this.Detaileurl = detaileirl;
            }
            public string Title { get; private set; }

            public string Brand { get; private set; }

            public string Sellday { get; private set; }

            public string Detaileurl { get; private set; }
        }

        public class Detaile {
            public Detaile(
                string title,
                string web,
                string brand,
                string sellday,
                string mainImage,
                string erogame,
                List<string> illustrators,
                List<string> scenarios,
                List<string> composers,
                List<string> voices,
                List<string> characters,
                List<string> singers,
                List<string> sampleCGs
            ) {
                this.Title = title;
                this.Web = web;
                this.Brand = brand;
                this.Sellday = sellday;
                this.MainImage = mainImage;
                this.Erogame = erogame;
                this.Illustrators = illustrators;
                this.Scenarios = scenarios;
                this.Composers = composers;
                this.Voices = voices;
                this.Characters = characters;
                this.Singers = singers;
                this.SampleCGs = sampleCGs;
            }

            public string Title { get; private set; }

            public string Web { get; private set; }

            public string Brand { get; private set; }

            public string Sellday { get; private set; }

            public string MainImage { get; private set; }

            public string Erogame { get; private set; }

            public List<string> Illustrators { get; private set; }

            public List<string> Scenarios { get; private set; }

            public List<string> Composers { get; private set; }

            public List<string> Voices { get; private set; }

            public List<string> Characters { get; private set; }

            public List<string> Singers { get; private set; }

            public List<string> SampleCGs { get; private set; }
        }

        public class GameDetaile {
            public GameDetaile(string hash, string executableFile, string saveFolder, Game.Detaile detaile, bool downloadComplete = false) {
                this.Hash = hash;
                this.ExecutableFile = executableFile;
                this.SaveFolder = saveFolder;
                this.DownloadComplete = downloadComplete;
                this.Detaile = detaile;
            }
            public string Hash { get; private set; }

            public string ExecutableFile { get; private set; }

            public string SaveFolder { get; private set; }

            public bool DownloadComplete { get; set; }

            public Detaile Detaile { get; private set; }
        }

        public static string GenerateHash(string gameTitle) {
            MD5 md5 = MD5.Create();
            var gameTitleBytes = Encoding.UTF8.GetBytes(gameTitle);
            var gameTitleMD5Bytes = md5.ComputeHash(gameTitleBytes);

            var stringBuilder = new StringBuilder();
            foreach (byte gameTitleMD5byte in gameTitleMD5Bytes) {
                stringBuilder.Append(gameTitleMD5byte.ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        public static async Task Downlaod(Game.GameDetaile detaile) {
            //MainImage Download
            try {
                await Task.Run(() => {
                    Folder.ExistsGameFolder(detaile.Hash);
                    var wevClient = new WebClient();
                    string url;
                    if (detaile.Detaile.MainImage.StartsWith("http", StringComparison.OrdinalIgnoreCase)) {
                        url = detaile.Detaile.MainImage;
                    } else {
                        url = "http:" + detaile.Detaile.MainImage;
                    }
                    wevClient.DownloadFile(
                        url,
                        AppPath.GamesFolder +
                        detaile.Hash + @"\" +
                        detaile.Hash + Path.GetExtension(detaile.Detaile.MainImage)
                    );
                    wevClient.Dispose();
                });

                //SampleCG Download
                await Task.Run(() => {
                    var count = 0;
                    var wevClient = new WebClient();
                    foreach (var sampleCG in detaile.Detaile.SampleCGs) {
                        string url;
                        if (sampleCG.StartsWith("http", StringComparison.OrdinalIgnoreCase)) {
                            url = sampleCG;
                        } else {
                            url = "http:" + sampleCG;
                        }
                        wevClient.DownloadFile(
                            url,
                            AppPath.GamesFolder +
                            detaile.Hash + @"\" +
                            @"sample\" +
                            "sample" + count.ToString() + Path.GetExtension(detaile.Detaile.MainImage)
                        );
                        count++;
                    }
                    wevClient.Dispose();
                });
            } catch {
                return;
			}
		}
    }
}
