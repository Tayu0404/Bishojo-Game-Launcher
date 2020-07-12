using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
