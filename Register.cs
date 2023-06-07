using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace Projekt_PV
{
	internal class Register
	{
		/// <summary>
		/// Třída pro registraci uživatele do databáze.
		/// </summary>
		/// <param name="writer">StreamWriter</param>
		/// <param name="reader">StreamReader</param>
		/// <param name="connectionString">string</param>
		/// <param name="username">string</param>
		public Register(StreamWriter writer, StreamReader reader, string connectionString, string username)
		{
			int number = 0;

			writer.WriteLine("Zadejte jméno nového žáka ('cancel' pro zrušení registrace): ");
			writer.Flush();
			string NewStudentfirstName = reader.ReadLine();

			if (NewStudentfirstName == "cancel") return;

			writer.WriteLine("Zadejte příjmení nového žáka ('cancel' pro zrušení registrace): ");
			writer.Flush();
			string NewStudentlastName = reader.ReadLine();

			if (NewStudentlastName == "cancel") return;

			string PotentionalUsername = NewStudentlastName.ToLower();
			PotentionalUsername = RemoveSpecialCharacters(PotentionalUsername);

			//Výběr třídy:
			writer.WriteLine("\n\r==== Výběr třídy pro registraci žáka ====");
			writer.WriteLine(ShowClasses(connectionString));
			writer.WriteLine("\n\r===========================================");
			writer.WriteLine("Zadejte třídu, do které chcete zapsat žáka ('cancel' pro návrat): ");
			writer.Write("\n\r" + username + "/register/" + ">");
			writer.Flush();
			string? classShortcut = reader.ReadLine();
			//První pímeno z classShortcut je vždycky velké a zbytek bude malý:
			classShortcut = classShortcut.Substring(0, 1).ToUpper() + classShortcut.Substring(1).ToLower();
			Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/register/" + ">" + classShortcut);

			if (classShortcut == "cancel") return;
			else if (!new CheckClass(connectionString, classShortcut).CheckClassInDatabase())
			{
				writer.WriteLine("\n\rTřída neexistuje!");
				return;
			}

			else
			{
				do
				{
					using (SqlConnection connection = new SqlConnection(connectionString))
					{
						connection.Open();
						using (SqlCommand command = new SqlCommand("SELECT * FROM uzivatele WHERE username = @PotentionalUsername", connection))
						{
							command.Parameters.AddWithValue("@PotentionalUsername", PotentionalUsername);
							using (SqlDataReader Sqlreader = command.ExecuteReader())
							{
								if (Sqlreader.HasRows)
								{
									if (number == 0)
									{
										PotentionalUsername += number.ToString();
										number++;
									}
									else
									{
										PotentionalUsername = PotentionalUsername.Remove(PotentionalUsername.Length - 1);
										PotentionalUsername += number.ToString();
										number++;
									}
								}
								else
								{
									break;
								}
							}
						}
					}
				} while (true);
			}

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("INSERT INTO uzivatele (username, firstName, lastName, isTeacher, trida) VALUES (@username, @firstName, @lastName, @isTeacher, @class)", connection))
				{
					command.Parameters.AddWithValue("@username", PotentionalUsername);
					command.Parameters.AddWithValue("@firstName", NewStudentfirstName);
					command.Parameters.AddWithValue("@lastName", NewStudentlastName);
					command.Parameters.AddWithValue("@isTeacher", false);
					command.Parameters.AddWithValue("@class", classShortcut);
					command.ExecuteNonQuery();
				}
			}

			writer.WriteLine("Žák " + NewStudentfirstName + " " + NewStudentlastName + " (" + PotentionalUsername + ") byl úspěšně přidán do školního systému.");
		}
		/// <summary>
		/// Metoda pro odstranění diakritiky ze stringu.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private string RemoveSpecialCharacters(string str) //Zdroj (Upraveno): https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net 
		{
			var filtered = str.Normalize(NormalizationForm.FormD).Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
			var newString = new String(filtered.ToArray());
			return newString;
		}
		/// <summary>
		/// Metoda pro zobrazení tříd.
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public string ShowClasses(string connectionString)
		{
			string classes = "";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM trida", connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								classes += "\n\r" + reader.GetString(0);
							}
						}
					}
				}
			}
			return classes;
		}
	}
}
