using System.Data.SqlClient;

namespace Projekt_PV
{
	internal class WriteGrade
	{
		/// <summary>
		/// Tato třída slouží pro zapsání známky do databáze.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="reader"></param>
		/// <param name="grades"></param>
		/// <param name="connectionString"></param>
		/// <param name="username"></param>
		public WriteGrade(StreamWriter writer, StreamReader reader, Grades grades, string connectionString, string username)
		{
			writer.WriteLine("\n\r==== Výběr předmětu pro zapsání známky =====");
			writer.WriteLine(grades.ShowSubjects());
			writer.WriteLine("\n\r============================================");
			writer.WriteLine("\n\rZadejte zkratku předmětu, do kterého chcete zapsat známku ('cancel' pro návrat): ");
			writer.Write(username + "/grades/write>");
			writer.Flush();
			string? subjectShortcut = reader.ReadLine();
			subjectShortcut = subjectShortcut?.ToUpper();
			Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/write>" + subjectShortcut);
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
				//Výběr třídy:
				writer.WriteLine("\n\r==== Výběr třídy pro zapsání známky ====");
				writer.WriteLine(grades.ShowClasses());
				writer.WriteLine("\n\r===========================================");
				writer.WriteLine("Zadejte třídu, do které chcete zapsat známku ('cancel' pro návrat): ");
				writer.Write("\n\r" + username + "/grades/write/" + subjectShortcut + ">");
				writer.Flush();
				string? classShortcut = reader.ReadLine();
				//První pímeno z classShortcut je vždycky velké a zbytek bude malý:
				classShortcut = classShortcut?.Substring(0, 1).ToUpper() + classShortcut?.Substring(1).ToLower();
				Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/write/" + subjectShortcut + ">" + classShortcut);
				if (classShortcut == "cancel")
				{
					return;
				}
				else if (classShortcut == null)
				{
					writer.WriteLine("Zadejte prosím třídu. Zkuste to znovu.");
					writer.Flush();
					return;
				}
				else if (!new CheckClass(connectionString, classShortcut).CheckClassInDatabase())
				{
					writer.WriteLine("Třída neexistuje. Zkuste to znovu.");
					writer.Flush();
					return;
				}
				else
				{
					//Výběr studenta:
					writer.WriteLine("\n\r==== Výběr studenta pro zapsání známky ====");
					writer.WriteLine(grades.ShowStudents(classShortcut));
					writer.WriteLine("\n\r===========================================");
					writer.WriteLine("Zadejte uživatelské jméno studenta, kterému chcete zapsat známku ('cancel' pro návrat): ");
					writer.Write("\n\r" + username + "/grades/write/" + subjectShortcut + ">");
					writer.Flush();
					string? studentUsername = reader.ReadLine();
					Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/write/" + subjectShortcut + "/" + classShortcut + ">" + studentUsername);
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
					else if (!new CheckStudent(connectionString, studentUsername).CheckStudentInDatabase())
					{
						writer.WriteLine("====== Student s uživatelským jménem '" + studentUsername + "' neexistuje. Zkuste to znovu. ======");
						writer.Flush();
						return;
					}
					else
					{
						writer.WriteLine("\n\rZadejte známku, kterou chcete zapsat ('cancel' pro návrat): ");
						writer.Write("\n\r" + username + "/grades/write/" + subjectShortcut + "/" + studentUsername + ">");
						writer.Flush();
						string? grade = reader.ReadLine();
						int gradeInt;
						Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/write/" + subjectShortcut + "/" + classShortcut + "/" + studentUsername + ">" + grade);
						if (grade == "cancel")
						{
							return;
						}
						else
						{
							try
							{
								gradeInt = Int32.Parse(grade);
							}
							catch (FormatException)
							{
								writer.WriteLine("\n\rZadejte prosím platnou známku v rozmezí 1-5. Zkuste to znovu.");
								writer.Flush();
								return;
							}
							if (gradeInt >= 1 && gradeInt <= 5)
							{
								writer.WriteLine("\n\rZadejte popis známky ('cancel' pro návrat): ");
								writer.Write("\n\r" + username + "/grades/write/" + subjectShortcut + "/" + studentUsername + "/" + grade + ">");
								writer.Flush();
								string? description = reader.ReadLine();
								Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/write/" + subjectShortcut + "/" + classShortcut + "/" + studentUsername + "/" + grade + ">" + description);
								if (description == "cancel")
								{
									return;
								}
								else
								{
									using (SqlConnection connection = new SqlConnection(connectionString))
									{
										connection.Open();
										SqlCommand command = new SqlCommand("INSERT INTO znamky (predmetZkratka, usernameZak, usernameUcitel, znamka, datum, description) VALUES (@predmetZkratka, @usernameZak, @usernameUcitel, @znamka, @datum, @description)", connection);

										command.Parameters.AddWithValue("@predmetZkratka", subjectShortcut);
										command.Parameters.AddWithValue("@usernameZak", studentUsername);
										command.Parameters.AddWithValue("@usernameUcitel", username);
										command.Parameters.AddWithValue("@znamka", gradeInt);
										command.Parameters.AddWithValue("@datum", DateTime.Now);
										command.Parameters.AddWithValue("@description", description);
										command.ExecuteNonQuery();
										writer.WriteLine("\n\r==== Známka byla úspěšně zapsána. ====");
										writer.Flush();
										return;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
