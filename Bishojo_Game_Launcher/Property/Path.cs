using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bishojo_Game_Launcher.Property {
	static class Path {
		public static string AppFolder {
			get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Bishojo Game Launcher\"; }
		}

		public static string GamesFolder {
			get { return AppFolder + @"games\"; }
		}
	}
}
