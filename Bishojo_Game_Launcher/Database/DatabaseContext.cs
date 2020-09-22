using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BishojoGameLauncher.Database {
	public class DatabaseContext {
		
		public DatabaseContext() {
			Database = new GameData();
			try {
				Read();
			} catch {
				
			}
		}

		private GameData _database;

		public GameData Database {
			get { return _database; }
			private set { _database = value; }
		}

		public string FilePath {
			get { return $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\Games.db"; }
		}

		public void Save() {
			try {
				Database.WriteXml(FilePath);
			}
			catch {
				throw;
			}
		}

		public void Read() {
			try {
				Database.ReadXml(FilePath);
			}
			catch {
				throw;
			}
		}
	}
}
