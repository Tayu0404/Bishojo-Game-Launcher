using BishojoGameLauncher.Configuration;
using BishojoGameLauncher.Game;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BishojoGameLauncher.Properties {
	[SettingsPath(DirectoryName = @".\conf\", FileName = "User.conf")]
	[DataContract]
	public class Settings : SettingsBase<Settings> {
		public static readonly Settings Instance = Load();

		[DataMember]
		public int GameListSortMode {
			get { return Get(a => a.GameListSortMode); }
			set { Set(a => a.GameListSortMode, value); }
		}

		[DataMember]
		public bool IsProxyEnable {
			get { return Get(a => a.IsProxyEnable); }
			set { Set(a => a.IsProxyEnable, value); }
		}

		[DataMember]
		public bool ProxyType {
			get { return Get(a => a.ProxyType); }
			set { Set(a => a.ProxyType, value); }
		}

		[DataMember]
		public string ProxyHost {
			get { return Get(a => a.ProxyHost); }
			set { Set(a => a.ProxyHost, value); }
		}

		[DataMember]
		public int ProxyPort {
			get { return Get(a => a.ProxyPort); }
			set { Set(a => a.ProxyPort, value); }
		}

		[DataMember]
		public string ProxyUser {
			get { return Get(a => a.ProxyUser); }
			set { Set(a => a.ProxyUser, value); }
		}

		[DataMember]
		public string ProxyPassword {
			get { return Get(a => a.ProxyPassword); }
			set { Set(a => a.ProxyPassword, value); }
		}
	}
}