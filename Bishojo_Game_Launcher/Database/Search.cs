using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BishojoGameLauncher.Database {
	class Search {
		public static GameData.GamesRow Hash(GameData gameData, string target) {
			var result = gameData.Games.AsEnumerable().Where(r => r.Hash == target).ToArray();
			return result[0];
		}
	}
}
