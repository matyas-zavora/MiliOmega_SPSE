using System.Data.SqlClient;

namespace Projekt_PV
{
	internal class EditGrade
	{
		int ID = 0;
		int[] PrimaryKeys;
		/// <summary>
		/// Výběr studenta, kterému chceme přepsat známku
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="reader"></param>
		/// <param name="grades"></param>
		/// <param name="connectionString"></param>
		/// <param name="username"></param>
		public EditGrade(StreamWriter writer, StreamReader reader, Grades grades, string connectionString, string username)
		{
			writer.WriteLine("\n\r==== Výběr předmětu pro přepsání známky =====");
			writer.WriteLine(grades.ShowSubjects());
			writer.WriteLine("\n\r=============================================");
			writer.WriteLine("\n\rZadejte zkratku předmětu, ze kterého chcete přepsat známku ('cancel' pro návrat): ");
			writer.Write(username + "/grades/edit>");
			writer.Flush();
			string subjectShortcut = reader.ReadLine();
			subjectShortcut = subjectShortcut.ToUpper();
			Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/edit>" + subjectShortcut);
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
		/// Výpis známek studenta
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
		/// <summary>
		/// Výběr studenta, kterému chceme přepsat známku
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="reader"></param>
		/// <param name="connectionString"></param>
		/// <param name="username"></param>
		/// <param name="subjectShortcut"></param>
		/// <param name="grades"></param>
		public void SelectStudentLoop(StreamWriter writer, StreamReader reader, string connectionString, string username, string subjectShortcut, Grades grades)
		{
			do
			{
				// Výběr třídy:
				writer.WriteLine("\n\r==== Výběr třídy pro přepsání známky ====");
				writer.WriteLine(grades.ShowClasses());
				writer.WriteLine("=============================================");
				writer.WriteLine("Zadejte zkratku třídy, ze které chcete přepsat známku ('cancel' pro návrat): ");
				writer.Write("\n\r" + username + "/grades/edit/" + subjectShortcut + ">");
				writer.Flush();
				string classShortcut = reader.ReadLine();
				Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/edit/" + subjectShortcut + ">" + classShortcut);
				//První písmeno velké, zbytek malé
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
					writer.WriteLine("\n\r==== Výběr studenta pro přepsání známky ====");
					writer.WriteLine(grades.ShowStudents(classShortcut));
					writer.WriteLine("================================================");
					writer.WriteLine("Zadejte uživatelské jméno studenta, kterému chcete přepsat známku ('cancel' pro návrat): ");
					writer.Write("\n\r" + username + "/grades/edit/" + subjectShortcut + "/" + classShortcut + ">");
					writer.Flush();
					string studentUsername = reader.ReadLine();
					Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/edit/" + subjectShortcut + "/" + classShortcut + ">" + studentUsername);
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
						writer.WriteLine("Zadaný student neexistuje. Zkuste to znovu.");
						writer.Flush();
						return;
					}
					else
					{
						//výběr známky:
						writer.WriteLine("\n\r==== Výběr známky pro přepsání ====");
						writer.WriteLine(ShowGrades(connectionString, username, subjectShortcut, studentUsername));
						writer.WriteLine("=======================================");
						writer.WriteLine("Zadejte známku, kterou chcete přepsat ('cancel' pro návrat): ");
						writer.Write("\n\r" + username + "/grades/edit/" + subjectShortcut + "/" + classShortcut + "/" + studentUsername + ">");
						writer.Flush();
						string gradeID = reader.ReadLine();
						Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/edit/" + subjectShortcut + "/" + classShortcut + "/" + studentUsername + ">" + gradeID);
						int gradeIDInt;
						bool success = Int32.TryParse(gradeID, out gradeIDInt);
						if (!success)
						{
							writer.WriteLine("Zadejte prosím platnou známku z výběru. Zkuste to znovu.");
							writer.Flush();
							return;
						}
						else if (gradeID == "cancel")
						{
							return;
						}
						else if (gradeID == null)
						{
							writer.WriteLine("Zadejte prosím známku. Zkuste to znovu.");
							writer.Flush();
							return;
						}
						else
						{
							//nová hodnota známky:
							writer.WriteLine("\n\r==== Výběr nové hodnoty známky ====");
							writer.WriteLine("Zadejte novou hodnotu známky ('cancel' pro návrat): ");
							writer.Write("\n\r" + username + "/grades/edit/" + subjectShortcut + "/" + classShortcut + "/" + studentUsername + "/" + gradeID + ">");
							writer.Flush();
							string gradeValue = reader.ReadLine();
							Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/edit/" + subjectShortcut + "/" + classShortcut + "/" + studentUsername + "/" + gradeID + ">" + gradeValue);
							if (gradeValue == "cancel")
							{
								return;
							}
							else if (gradeValue == null)
							{
								writer.WriteLine("Zadejte prosím známku. Zkuste to znovu.");
								writer.Flush();
								return;
							}
							else
							{

								//výběr popisu:
								writer.WriteLine("\n\r==== Výběr popisu pro přepsání ====");
								writer.WriteLine("Zadejte popis známky ('cancel' pro návrat): ");
								writer.Write("\n\r" + username + "/grades/edit/" + subjectShortcut + "/" + classShortcut + "/" + studentUsername + "/" + gradeID + "/" + gradeValue + ">");
								writer.Flush();
								string description = reader.ReadLine();
								Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/edit/" + subjectShortcut + "/" + classShortcut + "/" + studentUsername + "/" + gradeID + ">" + description);
								if (description == "cancel")
								{
									return;
								}
								else if (description == null)
								{
									writer.WriteLine("Zadejte prosím známku. Zkuste to znovu.");
									writer.Flush();
									return;
								}
								else
								{
									FinalEdit(subjectShortcut, classShortcut, studentUsername, gradeIDInt, description, connectionString, gradeValue);
									writer.WriteLine("Známka byla úspěšně přepsána.");
									writer.Flush();
									break;
								}
							}
						}
					}
				}
			} while (true);
		}
		/// <summary>
		/// Metoda pro přepsání známky.
		/// </summary>
		/// <param name="subjectShortcut"></param>
		/// <param name="classShortcut"></param>
		/// <param name="studentUsername"></param>
		/// <param name="gradeID"></param>
		/// <param name="description"></param>
		/// <param name="connectionString"></param>
		/// <param name="gradeValue"></param>
		private void FinalEdit(string subjectShortcut, string classShortcut, string studentUsername, int gradeID, string description, string connectionString, string gradeValue)
		{
			string sql = "UPDATE znamky SET predmetZkratka = @subjectShortcut, usernameZak = @studentUsername, znamka = @grade, description = @description, datum = @date WHERE ID = @ID";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@subjectShortcut", subjectShortcut);
					command.Parameters.AddWithValue("@studentUsername", studentUsername);
					command.Parameters.AddWithValue("@grade", gradeValue);
					command.Parameters.AddWithValue("@description", description);
					command.Parameters.AddWithValue("@date", DateTime.Now);
					command.Parameters.AddWithValue("@ID", PrimaryKeys[gradeID]);
					command.ExecuteNonQuery();
				}
			}
		}
	}
}