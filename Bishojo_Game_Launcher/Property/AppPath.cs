using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BishojoGameLauncher.Property {
	static class AppPath {
		public static string AppFolder {
			get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BishojoGameLauncher\"; }
		}

		public static string GamesFolder {
			get { return AppFolder + @"games\"; }
		}
	}
}
