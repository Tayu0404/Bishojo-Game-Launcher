using BishojoGameLauncher.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BishojoGameLauncher.Properties {
	class Sort {
		public static IOrderedEnumerable<KeyValuePair<string, GameDetaile>>TitleAscendingOrder(Dictionary<string, GameDetaile> games) {
			var sorted = games.OrderBy((x) => x.Value.Detaile.Title);
			return sorted;
		}

		public static IOrderedEnumerable<KeyValuePair<string, GameDetaile>> TitleDescendingOrder(Dictionary<string, GameDetaile> games) {
			var sorted = games.OrderByDescending((x) => x.Value.Detaile.Title);
			return sorted;
		}

		public static IOrderedEnumerable<KeyValuePair<string, GameDetaile>> BrandAscendingOrder(Dictionary<string, GameDetaile> games) {
			var sorted = games.OrderBy((x) => x.Value.Detaile.Brand);
			return sorted;
		}

		public static IOrderedEnumerable<KeyValuePair<string, GameDetaile>> BrandDescendingOrder(Dictionary<string, GameDetaile> games) {
			var sorted = games.OrderByDescending((x) => x.Value.Detaile.Brand);
			return sorted;
		}

		public static IOrderedEnumerable<KeyValuePair<string, GameDetaile>> SellDayAscendingOrder(Dictionary<string, GameDetaile> games) {
			var sorted = games.OrderBy((x) => x.Value.Detaile.Sellday);
			return sorted;
		}

		public static IOrderedEnumerable<KeyValuePair<string, GameDetaile>> SellDayDescendingOrder(Dictionary<string, GameDetaile> games) {
			var sorted = games.OrderByDescending((x) => x.Value.Detaile.Sellday);
			return sorted;
		}
	}
}