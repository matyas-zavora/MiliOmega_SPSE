using System.Data.SqlClient;

namespace Projekt_PV
{
	internal class Login
	{
		private string? username, password = null;
		private string connectionString = "Data Source=DESKTOP-QK8PH6N;Initial Catalog=PV_projekt;Integrated Security=True";
		public bool isTeacher;
		private StreamWriter writer;
		private StreamReader reader;
		/// <summary>
		/// Třída pro přihlášení nových clientů
		/// </summary>
		/// <param name="writer">StreamWriter</param>
		/// <param name="reader">StreamReader</param>
		public Login(StreamWriter writer, StreamReader reader)
		{
			this.writer = writer;
			this.reader = reader;
			StartLogin();
		}
		/// <summary>
		/// Spouštění přihlášení
		/// </summary>
		private void StartLogin()
		{
			while (true)
			{
				writer.WriteLine("\n\rZadejte své přihlašovací jméno: ");
				writer.Flush();
				this.username = reader.ReadLine().ToLower();

				if (username != null && CheckNewUser())
				{
					writer.WriteLine("\n\rVytvořte si heslo: ");
					writer.Flush();
					this.password = reader.ReadLine();
					writer.WriteLine("\n\rHeslo si prosím zapamatujte. Budete ho používát pro přístup do toho systému.");
					writer.Flush();
					writer.WriteLine(Greetings());
					writer.Flush();

					using (SqlConnection connection = new SqlConnection(connectionString))
					{
						connection.Open();
						using (SqlCommand command = new SqlCommand("UPDATE uzivatele SET _password = @password WHERE username = @username;", connection))
						{
							command.Parameters.AddWithValue("@username", username);
							command.Parameters.AddWithValue("@password", password);
							command.ExecuteNonQuery();
						}
					}

					break;
				}
				else
				{
					writer.WriteLine("\n\rZadejte své heslo: ");
					writer.Flush();
					this.password = reader.ReadLine();


					if (username == null || password == null)
					{
						writer.WriteLine("\n\rZadejte prosím Vaše uživatelské jméno i heslo. Zkuste to znovu.");
						writer.Flush();
						continue;
					}
					else if (CheckLogin(username, password))
					{
						writer.WriteLine("\n\rPřihlášení úspěšné.");
						writer.Flush();
						writer.WriteLine(Greetings());
						writer.Flush();
						break;
					}
					else
					{
						writer.WriteLine("\n\rPřihlášení se nezdařilo. Zkontrolujte své přihlašovací údaje. \n\r");
						writer.Flush();
						continue;
					}
				}
			}
		}
		/// <summary>
		/// Kontrola, zda-li je uživatel nový
		/// </summary>
		/// <param name="username">string</param>
		/// <param name="password">string</param>
		/// <returns></returns>
		private bool CheckLogin(string username, string password)
		{
			bool isAuthenticated = false;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM uzivatele WHERE username=@username AND _password=@password COLLATE Latin1_General_CS_AS", connection))
				{
					command.Parameters.AddWithValue("@username", username);
					command.Parameters.AddWithValue("@password", password);
					int result = (int)command.ExecuteScalar();
					if (result == 1)
					{
						isAuthenticated = true;
						this.isTeacher = CheckIfTeacher();
					}
				}
			}
			return isAuthenticated;
		}
		/// <summary>
		/// Kontrola, zda-li je uživatel učitel
		/// </summary>
		/// <returns></returns>
		private bool CheckIfTeacher()
		{
			bool isTeacher = false;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM uzivatele WHERE username=@username AND _password=@password AND isTeacher=1", connection))
				{
					command.Parameters.AddWithValue("@username", username);
					command.Parameters.AddWithValue("@password", password);
					int result = (int)command.ExecuteScalar();
					if (result == 1)
					{
						isTeacher = true;
					}
				}
			}
			return isTeacher;
		}
		/// <summary>
		/// String pro uvítání uživatele
		/// </summary>
		/// <returns></returns>
		private string Greetings()
		{
			return "\n\r====================================================================\n\rVítejte v systému SPŠE Ječná. Zadejte 'help' pro zobrazení nápovědy.\n\r====================================================================\n";
		}

		public string GetUsername()
		{
			return username;
		}
		public string GetPassword()
		{
			return password;
		}
		public bool GetIsTeacher()
		{
			return isTeacher;
		}
		/// <summary>
		/// Odhlášení uživatele
		/// </summary>
		public void Logout()
		{
			password = null;
			username = null;
			StartLogin();
		}
		public string GetFirstName()
		{
			string firstName = "";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT firstName FROM uzivatele WHERE username=@username AND _password=@password", connection))
				{
					command.Parameters.AddWithValue("@username", username);
					command.Parameters.AddWithValue("@password", password);
					firstName = (string)command.ExecuteScalar();
				}
			}
			return firstName;
		}
		public string GetLastName()
		{
			string lastName = "";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT lastName FROM uzivatele WHERE username=@username AND _password=@password", connection))
				{
					command.Parameters.AddWithValue("@username", username);
					command.Parameters.AddWithValue("@password", password);
					lastName = (string)command.ExecuteScalar();
				}
			}
			return lastName;
		}
		private bool CheckNewUser()
		{
			bool isNewUser = false;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM uzivatele WHERE username=@username AND _password IS NULL", connection))
				{
					command.Parameters.AddWithValue("@username", username);
					int result = (int)command.ExecuteScalar();
					if (result == 1)
					{
						isNewUser = true;
					}
				}
				return isNewUser;
			}
		}
		public string GetConnectionString()
		{
			return connectionString;
		}
	}
}
