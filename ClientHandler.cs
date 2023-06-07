namespace Projekt_PV
{
	internal class ClientHandler
	{
		private string question;
		private StreamWriter writer;
		private StreamReader reader;
		private bool isTeacher;
		private string connectionString, username;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="question">string</param>
		/// <param name="isTeacher">bool</param>
		/// <param name="connectionString">string</param>
		/// <param name="writer">StreamWriter</param>
		/// <param name="reader">StreamReader</param>
		/// <param name="username">string</param>
		public ClientHandler(string question, bool isTeacher, string connectionString, StreamWriter writer, StreamReader reader, string username)
		{
			this.question = question;
			this.isTeacher = isTeacher;
			this.connectionString = connectionString;
			this.writer = writer;
			this.reader = reader;
			this.username = username;
		}
		/// <summary>
		/// Odpoveď serveru na zadanou otázku
		/// </summary>
		public string Response()
		{
			string question = this.question.ToLower();
			string response = null;
			switch (question)
			{
				case "help": //Nápověda
					response = Help();
					break;
				case "register": //Registrace nového žáka (pouze pro učitele)
					if (isTeacher)
					{
						new Register(this.writer, this.reader, this.connectionString, this.username);
					}
					else
					{
						response = "Neznámý příkaz. Zadejte 'help' pro zobrazení nápovědy.";
					}
					break;
				case "logout": //Odhlášení
					response = "logout";
					break;
				case "grades":
					new Grades(this.writer, this.reader, this.connectionString, this.username, this.isTeacher);
					break;
				case "password":
					new ChangePassword(this.writer, this.reader, this.connectionString, this.username);
					response = "logout";
					break;
				default: //Neznámý příkaz
					response = "Neznámý příkaz. Zadejte 'help' pro zobrazení nápovědy.";
					break;
			}
			return response;
		}
		/// <summary>
		/// string s nápovědou
		/// </summary>
		/// <returns>string</returns>
		private string Help()
		{
			string help = "";
			if (!isTeacher)
			{
				help = "\n\rhelp - zobrazí nápovědu\n\rexit - ukončí program\n\rlogout - odhlášení ze systému (pro ukončení aplikace používat exit)\n\rgrades - přístup do systému známek\n\rpassword - změna hesla\n\r";
			}
			else
			{
				help = "\n\rhelp - zobrazí nápovědu\n\rexit - ukončí program\n\rregister - zaregistruje uživatele\n\rlogout - odhlášení ze systému (pro ukončení aplikace používat exit)\n\rgrades - přístup do systému známek\n\rpassword - změna hesla\n\r";
			}
			return help;
		}
	}
}
