using System.Data.SqlClient;

namespace Projekt_PV
{
	internal class DeleteGrade
	{
		int ID = 0;
		int[] PrimaryKeys;
		/// <summary>
		/// Konstruktor pro výběr předmětu, ze kterého chceme smazat známku
		/// </summary>
		/// <param name="writer">StreamWriter</param>
		/// <param name="reader">StreamReader</param>
		/// <param name="grades">Grades</param>
		/// <param name="connectionString">string</param>
		/// <param name="username">stribng</param>
		public DeleteGrade(StreamWriter writer, StreamReader reader, Grades grades, string connectionString, string username)
		{
			writer.WriteLine("\n\r==== Výběr předmětu pro smazání známky =====");
			writer.WriteLine(grades.ShowSubjects());
			writer.WriteLine("\n\r============================================");
			writer.WriteLine("\n\rZadejte zkratku předmětu, ze kterého chcete smazat známku ('cancel' pro návrat): ");
			writer.Write(username + "/grades/delete>");
			writer.Flush();
			string subjectShortcut = reader.ReadLine();
			subjectShortcut = subjectShortcut.ToUpper();
			Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/delete>" + subjectShortcut);
			if (subjectShortcut == "cancel")
			{
				return;
			}
			else if (subjectShortcut == null)
			{
				writer.WriteLine("Zadejte prosím zkratku předmětu. Zkuste to znovu.");
				writer.Flush();
				return;
			}
			else
			{
				//Kontrola, zda předmět existuje
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand("SELECT * FROM predmety WHERE zkratka = '" + subjectShortcut + "'", connection))
					{
						using (SqlDataReader SQLreader = command.ExecuteReader())
						{
							if (!SQLreader.HasRows)
							{
								writer.WriteLine("Zadaný předmět neexistuje. Zkuste to znovu.");
								writer.Flush();
								return;
							}
							else
							{
								SelectStudentLoop(writer, reader, connectionString, username, subjectShortcut, grades);
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Metoda pro výběr studenta, kterému chceme smazat známku
		/// </summary>
		public void SelectStudentLoop(StreamWriter writer, StreamReader reader, string connectionString, string username, string subjectShortcut, Grades grades)
		{
			do
			{
				writer.WriteLine("\n\r==== Výběr třídy pro smazání známky ====");
				writer.WriteLine(grades.ShowClasses());
				writer.WriteLine("===========================================");
				writer.WriteLine("Zadejte zkratku třídy, ze které chcete smazat známku ('cancel' pro návrat): ");
				writer.Write("\n\r" + username + "/grades/delete/" + subjectShortcut + ">");
				writer.Flush();
				string classShortcut = reader.ReadLine();
				Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/delete/" + subjectShortcut + ">" + classShortcut);
				classShortcut = classShortcut.Substring(0, 1).ToUpper() + classShortcut.Substring(1).ToLower();
				if (classShortcut == "cancel")
				{
					return;
				}
				else if (classShortcut == null)
				{
					writer.WriteLine("Zadejte prosím zkratku třídy. Zkuste to znovu.");
					writer.Flush();
					return;
				}
				else if (!new CheckClass(connectionString, classShortcut).CheckClassInDatabase())
				{
					writer.WriteLine("Zadaná třída neexistuje. Zkuste to znovu.");
					writer.Flush();
					return;
				}
				else
				{
					writer.WriteLine("\n\r==== Výběr studenta pro smazání známky ====");
					writer.WriteLine(grades.ShowStudents(classShortcut));
					writer.WriteLine("===========================================");
					writer.WriteLine("Zadejte uživatelské jméno studenta, kterému chcete smazat známku ('cancel' pro návrat): ");
					writer.Write("\n\r" + username + "/grades/delete/" + subjectShortcut + ">");
					writer.Flush();
					string studentUsername = reader.ReadLine();
					Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/delete/" + subjectShortcut + "/" + classShortcut + ">" + studentUsername);
					if (studentUsername == "cancel")
					{
						return;
					}
					else if (studentUsername == null)
					{
						writer.WriteLine("Zadejte prosím uživatelské jméno studenta. Zkuste to znovu.");
						writer.Flush();
						return;
					}
					else
					{
						writer.WriteLine("\n\r==== Výběr známky pro smazání ====");
						writer.WriteLine(ShowGrades(connectionString, username, subjectShortcut, studentUsername));
						writer.WriteLine("\n\r==================================");
						writer.WriteLine("\n\rZadejte číslo známky, kterou chcete smazat ('cancel' pro návrat): ");
						writer.Write("\n\r" + username + "/grades/delete/" + subjectShortcut + "/" + studentUsername + ">");
						writer.Flush();
						string response = reader.ReadLine();
						int responseInt;
						Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/delete/" + subjectShortcut + "/" + classShortcut + "/" + studentUsername + ">" + response);
						if (response == "cancel")
						{
							break;
						}
						else
						{
							try
							{
								responseInt = Int32.Parse(response);
							}
							catch (FormatException)
							{
								writer.WriteLine("\n\rZadejte prosím platné číslo známky z výběru výše. Zkuste to znovu.");
								writer.Flush();
								return;
							}
							if (responseInt >= 0 && responseInt <= this.ID)
							{

								using (SqlConnection connection = new SqlConnection(connectionString))
								{
									connection.Open();
									using (SqlCommand command = new SqlCommand("DELETE FROM znamky WHERE id = @id", connection))
									{
										command.Parameters.AddWithValue("@id", PrimaryKeys[responseInt]);
										command.ExecuteNonQuery();
										writer.WriteLine("\n\r==== Známka byla úspěšně smazána. ====");
										writer.Flush();
										return;
									}
								}
							}
							else
							{
								writer.WriteLine("\n\rZadejte prosím platné číslo známky z výběru výše. Zkuste to znovu.");
								writer.Flush();
								return;
							}
						}
					}
				}
			} while (true);
		}
		/// <summary>
		/// Vypíše známky studenta z databáze.
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="username"></param>
		/// <param name="subjectShortcut"></param>
		/// <param name="studentUsername"></param>
		/// <returns></returns>
		public string ShowGrades(string connectionString, string username, string subjectShortcut, string studentUsername)
		{
			string result = "";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM znamky WHERE usernameUcitel = @usernameUcitel AND predmetZkratka = @subjectShortcut AND usernameZak = @studentUsername", connection))
				{
					command.Parameters.AddWithValue("@usernameUcitel", username);
					command.Parameters.AddWithValue("@subjectShortcut", subjectShortcut);
					command.Parameters.AddWithValue("@studentUsername", studentUsername);
					command.ExecuteNonQuery();
					using (SqlDataReader reader = command.ExecuteReader())
					{
						PrimaryKeys = new int[reader.FieldCount + 1];
						while (reader.Read())
						{
							result += "[" + ID + "]" + " " + reader["predmetZkratka"].ToString() + " " + reader["usernameZak"].ToString() + " " + reader["usernameUcitel"].ToString() + " " + reader["znamka"].ToString() + " " + reader["datum"].ToString() + "\n\r";
							PrimaryKeys[ID] = (int)reader["ID"];
							this.ID++;
						}
					}
				}
			}
			return result;
		}
	}
}
