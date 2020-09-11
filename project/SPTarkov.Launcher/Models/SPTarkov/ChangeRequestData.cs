﻿namespace SPTarkov.Launcher
{
	public struct ChangeRequestData
	{
		public string email;
		public string password;
		public string change;

		public ChangeRequestData(string email, string password, string change)
		{
			this.email = email;
			this.password = password;
			this.change = change;
		}
	}
}
