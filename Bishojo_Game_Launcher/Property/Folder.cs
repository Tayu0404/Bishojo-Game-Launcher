using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bishojo_Game_Launcher.Property {
	class Folder {
		public static void ExistsAllAppFolder() {
			if (!Directory.Exists(Path.AppFolder)) {
				CreateAppFolder();
			}
			if (!Directory.Exists(Path.GamesFolder)) {
				CreateGamesFolder();
			}
		}

		public static void CreateAppFolder() {
			Directory.CreateDirectory(Path.AppFolder);
		}

		public static void CreateGamesFolder() {
			Directory.CreateDirectory(Path.GamesFolder);
		}

		public static void CreateGameFolder(string gameTitleMD5) {
			Directory.CreateDirectory(Path.GamesFolder + gameTitleMD5 + @"\");
		}
	}
}
