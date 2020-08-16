using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BishojoGameLauncher.Game {
	class Generate {
        public static string Hash(string gameTitle) {
            MD5 md5 = MD5.Create();
            var gameTitleBytes = Encoding.UTF8.GetBytes(gameTitle);
            var gameTitleMD5Bytes = md5.ComputeHash(gameTitleBytes);

            var stringBuilder = new StringBuilder();
            foreach (byte gameTitleMD5byte in gameTitleMD5Bytes) {
                stringBuilder.Append(gameTitleMD5byte.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }

    class SearchResult {
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

    [DataContract]
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

        [DataMember]
        public string Title { get; private set; }

        [DataMember]
        public string Web { get; private set; }

        [DataMember]
        public string Brand { get; private set; }

        [DataMember]
        public string Sellday { get; private set; }

        [DataMember]
        public string MainImage { get; private set; }

        [DataMember]
        public string Erogame { get; private set; }

        [DataMember]
        public List<string> Illustrators { get; private set; }

        [DataMember]
        public List<string> Scenarios { get; private set; }

        [DataMember]
        public List<string> Composers { get; private set; }

        [DataMember]
        public List<string> Voices { get; private set; }

        [DataMember]
        public List<string> Characters { get; private set; }

        [DataMember]
        public List<string> Singers { get; private set; }

        [DataMember]
        public List<string> SampleCGs { get; private set; }
    }

    [DataContract]
    public class GameDetaile {
        public GameDetaile(
            string hash,
            string executableFile,
            string saveFolder,
            Detaile detaile,
            bool downloadComplete = false,
            string customIconPath = ""
        ) {
            this.Hash = hash;
            this.ExecutableFile = executableFile;
            this.SaveFolder = saveFolder;
            this.DownloadComplete = downloadComplete;
            this.CustomaIconPath = customIconPath;
            this.Detaile = detaile;
        }
        
        [DataMember]
        public string Hash { get; private set; }

        [DataMember]
        public string ExecutableFile { get; private set; }

        [DataMember]
        public string SaveFolder { get; private set; }

        [DataMember]
        public bool DownloadComplete { get; set; }

        [DataMember]
        public string CustomaIconPath { get; set; }

        [DataMember]
        public Detaile Detaile { get; private set; }
    }
}
