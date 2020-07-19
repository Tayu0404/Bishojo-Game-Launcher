using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bishojo_Game_Launcher.Property {
	class Folder {
		public static void ExistsAllAppFolder() {
			if (!Directory.Exists(AppPath.AppFolder)) {
				CreateAppFolder();
			}
			if (!Directory.Exists(AppPath.GamesFolder)) {
				CreateGamesFolder();
			}
		}

		public static void CreateAppFolder() {
			Directory.CreateDirectory(AppPath.AppFolder);
		}

		public static void CreateGamesFolder() {
			Directory.CreateDirectory(AppPath.GamesFolder);
		}

		public static void CreateGameFolder(string gameTitleMD5) {
			var gameFolderPath = AppPath.GamesFolder + gameTitleMD5 + @"\";
			Directory.CreateDirectory(gameFolderPath);
			Directory.CreateDirectory(gameFolderPath + @"sample\");
		}

		public static void ExistsAppFolder() {
			if (!Directory.Exists(AppPath.AppFolder)) {
				CreateAppFolder();
			}
		}

		public static void ExistsGamesFolder() {
			if (!Directory.Exists(AppPath.GamesFolder)) {
				CreateGamesFolder();
			}
		}

		public static void ExistsGameFolder(string gameTitleMD5) {
			if (!Directory.Exists(AppPath.GamesFolder + gameTitleMD5 + @"\")) {
				CreateGameFolder(gameTitleMD5);
			}
		}
	}
}
