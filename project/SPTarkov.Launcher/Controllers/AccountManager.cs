using SPTarkov.Common.Utils.App;
using SPTarkov.Launcher.Helpers;
using SPTarkov.Launcher.Models.Launcher;

namespace SPTarkov.Launcher
{
    public static class AccountManager
	{
        private const string STATUS_FAILED = "FAILED";
        private const string STATUS_OK = "OK";

		public static AccountInfo SelectedAccount { get; private set; } = null;

		public static int Login(LoginModel Creds)
        {
			return Login(Creds.Email, Creds.Password);
        }

		public static int Login(string email, string password)
		{
			LoginRequestData data = new LoginRequestData(email, password);
			string id = STATUS_FAILED;
			string json = "";

			try
			{
				id = RequestHandler.RequestLogin(data);

				if (id == STATUS_FAILED)
				{
					return -1;
				}

				json = RequestHandler.RequestAccount(data);
			}
			catch
			{
				return -2;
			}

			SelectedAccount = Json.Deserialize<AccountInfo>(json);
            RequestHandler.ChangeSession(SelectedAccount.id);

            return 1;
		}

		public static int Register(string email, string password, string edition)
		{
			RegisterRequestData data = new RegisterRequestData(email, password, edition);
			string registerStatus = STATUS_FAILED;

			try
			{
				registerStatus = RequestHandler.RequestRegister(data);

				if (registerStatus != STATUS_OK)
				{
					return -1;
				}
			}
			catch
			{
				return -2;
			}

			int loginStatus = Login(email, password);

			if (loginStatus != 1)
			{
				switch (loginStatus)
				{
					case -1:
						return -3;

					case -2:
						return -2;
				}
			}

			return 1;
		}

		public static int Remove()
		{
			LoginRequestData data = new LoginRequestData(SelectedAccount.email, SelectedAccount.password);
			string json = STATUS_FAILED;

			try
			{
				json = RequestHandler.RequestAccount(data);

				if (json != STATUS_OK)
				{
					return -1;
				}
			}
			catch
			{
				return -1;
			}

			SelectedAccount = null;


			// Left in for future, incase needed for reference
			//launcherConfig.Email = "";
			//launcherConfig.Password = "";
			//JsonHandler.SaveLauncherConfig(launcherConfig);
			return 1;
		}

		public static int ChangeEmail(string email)
		{
			ChangeRequestData data = new ChangeRequestData(SelectedAccount.email, SelectedAccount.password, email);
			string json = STATUS_FAILED;

			try
			{
				json = RequestHandler.RequestChangeEmail(data);

				if (json != STATUS_OK)
				{
					return -1;
				}
			}
			catch
			{
				return -2;
			}

			ServerSetting DefaultServer = LauncherSettingsProvider.GetDefaultServer();

			DefaultServer.AutoLoginCreds.Email = email;
            SelectedAccount.email = email;
			LauncherSettingsProvider.Instance.SaveSettings();

            return 1;
		}

		public static int ChangePassword(string password)
		{
			ChangeRequestData data = new ChangeRequestData(SelectedAccount.email, SelectedAccount.password, password);
			string json = STATUS_FAILED;

			try
			{
				json = RequestHandler.RequestChangePassword(data);

				if (json != STATUS_OK)
				{
					return -1;
				}
			}
			catch
			{
				return -2;
			}

			ServerSetting DefaultServer = LauncherSettingsProvider.GetDefaultServer();

			DefaultServer.AutoLoginCreds.Password = password;
            SelectedAccount.password = password;
            LauncherSettingsProvider.Instance.SaveSettings();

			return 1;
		}

		public static int Wipe(string edition)
		{
			RegisterRequestData data = new RegisterRequestData(SelectedAccount.email, SelectedAccount.password, edition);
			string json = STATUS_FAILED;

			try
			{
				json = RequestHandler.RequestWipe(data);

				if (json != STATUS_OK)
				{
					return -1;
				}
			}
			catch
			{
				return -2;
			}

			SelectedAccount.edition = edition;
			return 1;
		}
	}
}
