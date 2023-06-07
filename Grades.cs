using System.Data.SqlClient;

namespace Projekt_PV
{
	internal class Grades
	{
		private StreamWriter writer;
		private StreamReader reader;
		private string connectionString;
		private string username;
		private bool isTeacher;
		private bool isRunning = true;
		/// <summary>
		/// Zapnutí smyčky pro známkování
		/// </summary>
		/// <param name="writer">StreamWriter</param>
		/// <param name="reader">StreamReader</param>
		/// <param name="connectionString">string</param>
		/// <param name="username">string</param>
		/// <param name="isTeacher">bool</param>
		public Grades(StreamWriter writer, StreamReader reader, string connectionString, string username, bool isTeacher)
		{
			this.writer = writer;
			this.reader = reader;
			this.connectionString = connectionString;
			this.username = username;
			this.isTeacher = isTeacher;
			if (isTeacher)
			{
				GradesLoopTeacher();
			}
			else
			{
				GradesLoop();
			}
		}
		/// <summary>
		/// Smyčka známkování pro učitele
		/// </summary>
		private void GradesLoopTeacher()
		{
			while (isRunning)
			{
				writer.WriteLine("\n\rNabídka dostupných příkazů:\n\rwrite - zapsat známku studentovi\n\redit - přepsat známku studentovi\n\rdelete - smazat známku studentovi\n\rshow - zobrazit studentovi známky\n\rcancel - návrat do hlavního menu");
				writer.Write("\n\r" + this.username + "/grades>");
				writer.Flush();
				string data = reader.ReadLine();
				data = data.ToLower();
				Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + this.username + "/grades>" + data);

				switch (data)
				{
					case "write":
						new WriteGrade(writer, reader, this, connectionString, this.username);
						break;
					case "edit":
						new EditGrade(this.writer, this.reader, this, this.connectionString, this.username);
						break;
					case "delete":
						new DeleteGrade(this.writer, this.reader, this, this.connectionString, this.username);
						break;
					case "show":
						new ShowGrades(this.writer, this.reader, this, this.connectionString, this.username);
						break;
					case "cancel":
						isRunning = false;
						break;
					default:
						writer.WriteLine("Neznámý příkaz. Zkuste to znovu.");
						writer.Flush();
						break;
				}
			}
		}
		/// <summary>
		/// Zobrazení studentů podle třídy
		/// </summary>
		/// <param name="classShortcut">string?</param>
		/// <returns></returns>
		public string ShowStudents(string? classShortcut)
		{
			string students = "";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT username, firstName, lastName FROM uzivatele WHERE trida = @classShortcut AND isTeacher = 0", connection))
				{
					command.Parameters.AddWithValue("@classShortcut", classShortcut);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								students += "\n\r" + reader.GetString(0) + " - " + reader.GetString(1) + " " + reader.GetString(2);
							}
						}
					}
				}
			}
			return students;
		}
		/// <summary>
		/// Smyčka známkování pro studenta
		/// </summary>
		private void GradesLoop()
		{
			while (isRunning)
			{
				writer.WriteLine("\n\r==== Výběr předmětu pro vypsání známek =====");
				writer.WriteLine(ShowSubjects());
				writer.WriteLine("\n\r============================================");
				writer.WriteLine("\n\n\rZadejte zkratku předmětu, ze kterého chcete zobrazit známky ('cancel' pro návrat): ");
				writer.Write(this.username + "/grades>");
				writer.Flush();
				string subjectShortcut = reader.ReadLine();
				Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + this.username + "/grades>" + subjectShortcut);
				if (subjectShortcut == "cancel")
				{
					isRunning = false;
					break;
				}
				else if (subjectShortcut == null)
				{
					writer.WriteLine("Zadejte prosím zkratku předmětu. Zkuste to znovu.");
					writer.Flush();
				}
				else
				{
					ShowGrades(subjectShortcut, this.username);
				}
			}
		}
		/// <summary>
		/// Výpis všech předmětů z databáze 
		/// </summary>
		/// <returns></returns>
		public string ShowSubjects()
		{
			string subjects = "";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM predmety", connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								subjects += "\n\r" + reader.GetString(0) + " - " + reader.GetString(1);
							}
						}
					}
				}
			}
			return subjects;
		}
		/// <summary>
		/// Výpis známek studenta z předmětu
		/// </summary>
		/// <param name="subjectShortcut"></param>
		/// <param name="studentUsername"></param>
		public void ShowGrades(string subjectShortcut, string studentUsername)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT nazev FROM predmety WHERE zkratka = @subjectShortcut", connection))
				{
					command.Parameters.AddWithValue("@subjectShortcut", subjectShortcut);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								writer.WriteLine("\n\rZnámky z předmětu " + reader.GetString(0) + ": ");
								writer.Flush();
							}
							writer.WriteLine("============================================");
							writer.Flush();
						}
						else
						{
							writer.WriteLine("Předmět se zktratkou " + subjectShortcut + " neexistuje.");
							writer.Flush();

						}

					}
				}
			}

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT znamka, datum, usernameUcitel, description FROM znamky WHERE predmetZkratka = @subjectShortcut AND usernameZak = @usernameZak", connection))
				{
					command.Parameters.AddWithValue("@subjectShortcut", subjectShortcut);
					command.Parameters.AddWithValue("@usernameZak", studentUsername);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								string teacherName = "";
								using (SqlConnection connection2 = new SqlConnection(connectionString))
								{
									connection2.Open();
									using (SqlCommand command2 = new SqlCommand("SELECT firstName, lastName FROM uzivatele WHERE username = @username", connection2))
									{
										command2.Parameters.AddWithValue("@username", reader.GetString(2));
										using (SqlDataReader reader2 = command2.ExecuteReader())
										{
											if (reader2.HasRows)
											{
												while (reader2.Read())
												{
													teacherName = reader2.GetString(0) + " " + reader2.GetString(1);
												}
											}
										}
									}
								}
								writer.WriteLine("Známka: " + reader.GetByte(0) + " Datum: " + reader.GetDateTime(1).ToString("dd.MM.yyyy") + " Učitel: " + teacherName + " Popis: " + reader.GetString(3));
								writer.Flush();
							}
						}
					}
				}
				writer.WriteLine("============================================");
				writer.Flush();
			}
		}
		/// <summary>
		/// Výpis všech tříd z databáze
		/// </summary>
		/// <returns></returns>
		public string ShowClasses()
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
