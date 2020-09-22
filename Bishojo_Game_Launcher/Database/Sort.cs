using BishojoGameLauncher.Game;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BishojoGameLauncher.Database {
	class Sort {

		public static List<GameData.GamesRow> TitleAscendingOrder(GameData gameData){
			var result = gameData.Games.AsEnumerable().OrderBy(r => r.Table).ToList();
			return result;
		}

		public static List<GameData.GamesRow> TitleDescendingOrder(GameData gameData) {
			var result = gameData.Games.AsEnumerable().OrderByDescending(r => r.Table).ToList();
			return result;
		}

		public static List<GameData.GamesRow> BrandAscendingOrder(GameData gameData) {
			var result = gameData.Games.AsEnumerable().OrderBy(r => r.Brand).ToList();
			return result;
		}

		public static List<GameData.GamesRow> BrandDescendingOrder(GameData gameData) {
			var result = gameData.Games.AsEnumerable().OrderByDescending(r => r.Brand).ToList();
			return result;
		}

		public static List<GameData.GamesRow> SellDayAscendingOrder(GameData gameData) {
			var result = gameData.Games.AsEnumerable().OrderBy(r => r.Sellday).ToList();
			return result;
		}

		public static List<GameData.GamesRow> SellDayDescendingOrder(GameData gameData) {
			var result = gameData.Games.AsEnumerable().OrderByDescending(r => r.Sellday).ToList();
			return result;
		}
	}
}