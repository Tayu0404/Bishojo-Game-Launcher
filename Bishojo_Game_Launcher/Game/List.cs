using Bishojo_Game_Launcher.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bishojo_Game_Launcher.Game {
	class List {
		static List<GameDetaile> games;
		public class GameDetaile {
			public GameDetaile(string hash, Game.Detaile detaile) {
				this.Hash = hash;
				this.Detaile = detaile;
			}
			public string Hash { get; private set; }
			public Game.Detaile Detaile { get; private set; }
		}

		public List<GameDetaile> Games {
			get { return games; }
			private set { games = value; }
		}

		public void Write() {
			var boxedLunchRows = this.Games.Select(x =>
				new XElement("Game",
					new XElement("Hash", x.Hash),
					new XElement("Detaile",
						new XElement("Title", x.Detaile.Title),
						new XElement("Web", x.Detaile.Web),
						new XElement("Brand", x.Detaile.Brand),
						new XElement("Sellday", x.Detaile.Sellday),
						new XElement("MainImage", x.Detaile.MainImage),
						new XElement("Erogame", x.Detaile.Erogame),
						new XElement("Illustrators", x.Detaile.Illustrators.Select(m =>
							new XElement("Illustrator", m)
						)),
						new XElement("Scenarios", x.Detaile.Scenarios.Select(m =>
							new XElement("Scenario", m)
						)),
						new XElement("Composers", x.Detaile.Composers.Select(m =>
							new XElement("Composer", m)
						)),
						new XElement("Voices", x.Detaile.Voices.Select(m =>
							new XElement("Voice", m)
						)),
						new XElement("Characters", x.Detaile.Characters.Select(m =>
							new XElement("Character", m)
						)),
						new XElement("Singers", x.Detaile.Singers.Select(m =>
							new XElement("Singer", m)
						)),
						new XElement("SampleCGs", x.Detaile.SampleCGs.Select(m =>
							new XElement("SampleCG", m)
						))
					)
				)
			);
			var xElement = new XElement("Games", boxedLunchRows);
			var xDocument = new XDocument(xElement);
			xDocument.Save(Path.GamesFolder + @"list.xml");
		}

		public void Add(string hash, Game.Detaile detaile) {
			var gameDetaile = new GameDetaile(hash, detaile);
			this.Games.Add(gameDetaile);
		}
	}
}
