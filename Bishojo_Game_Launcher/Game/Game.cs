using System.Security.Cryptography;
using System.Text;


namespace Bishojo_Game_Launcher.Game {
	class Game {
		public void Add(string gameTitle) {
			var gameTitleMD5 = generateHash(gameTitle);
		}
		private string generateHash(string gameTitle) {
			MD5 md5 = MD5.Create();
			var gameTitleBytes = Encoding.UTF8.GetBytes(gameTitle);
			var gameTitleMD5Bytes = md5.ComputeHash(gameTitleBytes);

			var stringBuilder = new StringBuilder();
			foreach (byte gameTitleMD5byte in gameTitleMD5Bytes) {
				stringBuilder.Append(gameTitleMD5byte.ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}
