using AngleSharp.Dom;
using AngleSharp.Html.Dom.Events;
using Bishojo_Game_Launcher.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bishojo_Game_Launcher.Game {
	class List {
		static List<GameDetaile> games;

		public List() {
			games = new List<GameDetaile>();
		}

		public class GameDetaile {
			public GameDetaile(string hash, string executableFile, string saveFolder, Game.Detaile detaile) {
				this.Hash = hash;
				this.ExecutableFile = executableFile;
				this.SaveFolder = saveFolder;
				this.Detaile = detaile;
			}
			public string Hash { get; private set; }

			public string ExecutableFile { get; private set; }

			public string SaveFolder { get; private set; }

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
					new XElement("ExecutableFile", x.ExecutableFile),
					new XElement("SaveFolder", x.SaveFolder),
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

		public void Read() {
			try {
				var xDocument = XDocument.Load(Path.GamesFolder + @"list.xml");
				var xElements = xDocument.Root.Elements();
				foreach(var xElement in xElements) {
					var hash = xElement.Element("Hash").Value;
					var executableFile = xElement.Element("ExecutableFile").Value;
					var saveFolder = xElement.Element("SaveFolder").Value;
					var detaile = xElement.Element("Detaile");
					var title = detaile.Element("Title").Value;
					var web = detaile.Element("Web").Value;
					var brand = detaile.Element("Brand").Value;
					var sellday  = detaile.Element("Sellday").Value;
					var mainImage = detaile.Element("MainImage").Value;
					var erogame = detaile.Element("Erogame").Value;
					var illustrators = detaile.Element("Illustrators").Elements("Illustrator").Select(element =>
						element.Value
					).ToList();
					var scenarios = detaile.Element("Scenarios").Elements("Scenario").Select(element =>
						element.Value
					).ToList();
					var composers = detaile.Element("Composers").Elements("Composer").Select(element =>
						element.Value
					).ToList();
					var voices = detaile.Element("Voices").Elements("Voice").Select(element =>
						element.Value
					).ToList();
					var characters = detaile.Element("Characters").Elements("Character").Select(element =>
						element.Value
					).ToList();
					var singers = detaile.Element("Singers").Elements("Singer").Select(element =>
						element.Value
					).ToList();
					var sampleCGs = detaile.Element("SampleCGs").Elements("SampleCG").Select(element =>
						element.Value
					).ToList();
					this.Add(
						hash,
						executableFile,
						saveFolder,
						new Game.Detaile(
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
						)
					);
				}
			} catch {
				return;
			}
		}

		public void Add(string hash, string executableFile, string saveFolder, Game.Detaile detaile) {
			var gameDetaile = new GameDetaile(hash, executableFile, saveFolder, detaile);
			games.Add(gameDetaile);
		}
	}
}
