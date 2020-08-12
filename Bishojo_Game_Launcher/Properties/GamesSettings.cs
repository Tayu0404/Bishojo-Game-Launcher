using BishojoGameLauncher.Configuration;
using BishojoGameLauncher.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BishojoGameLauncher.Properties {
	[SettingsPath(FileName = "Games.conf")]
	[DataContract]
	public class GamesSettings : SettingsBase<GamesSettings> {
		public static readonly GamesSettings Instance = Load();

		[DataMember]
		public Dictionary<string, GameDetaile> Games {
			get { return Get(a => a.Games); }
			set { Set(a => a.Games, value);}
		}
	}
}
