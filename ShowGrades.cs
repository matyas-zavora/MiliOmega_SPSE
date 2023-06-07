namespace Projekt_PV
{
	internal class ShowGrades
	{
		/// <summary>
		/// Třída pro zobrazení známek studenta.
		/// </summary>
		/// <param name="writer">StreamWriter</param>
		/// <param name="reader">StreamReader</param>
		/// <param name="grades">Grades</param>
		/// <param name="connectionString">string</param>
		/// <param name="username">string</param>
		public ShowGrades(StreamWriter writer, StreamReader reader, Grades grades, string connectionString, string username)
		{
			writer.WriteLine("\n\r==== Výběr předmětu pro zobrazení známek =====");
			writer.WriteLine(grades.ShowSubjects());
			writer.WriteLine("\n\r============================================");
			writer.WriteLine("\n\rZadejte zkratku předmětu, který chcete zobrazit ('cancel' pro návrat): ");
			writer.Write(username + "/grades/show>");
			writer.Flush();
			string subjectShortcut = reader.ReadLine();
			subjectShortcut = subjectShortcut.ToUpper();
			Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/show>" + subjectShortcut);

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
				writer.WriteLine("\n\r==== Výběr třídy pro zobrazení známek ====");
				writer.WriteLine(grades.ShowClasses());
				writer.WriteLine("\n\r===========================================");
				writer.WriteLine("Zadejte třídu, kterou chcete zobrazit ('cancel' pro návrat): ");
				writer.Write("\n\r" + username + "/grades/show/" + subjectShortcut + ">");
				writer.Flush();
				string? classShortcut = reader.ReadLine();
				classShortcut = classShortcut?.Substring(0, 1).ToUpper() + classShortcut?.Substring(1).ToLower();
				Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/show/" + subjectShortcut + ">" + classShortcut);

				if (!new CheckClass(connectionString, classShortcut).CheckClassInDatabase())
				{
					writer.WriteLine("Třída neexistuje. Zkuste to znovu.");
					writer.Flush();
					return;
				}
				else if (classShortcut == null)
				{
					writer.WriteLine("Zapište prosím název třídy. Zkuste to znovu.");
					writer.Flush();
					return;
				}
				else
				{
					writer.WriteLine("\n\r==== Výběr studenta pro zobrazení známek ====");
					writer.WriteLine(grades.ShowStudents(classShortcut));
					writer.WriteLine("\n\r===========================================");
					writer.WriteLine("Zadejte uživatelské jméno studenta, kterého chcete zobrazit ('cancel' pro návrat): ");
					writer.Write("\n\r" + username + "/grades/show/" + subjectShortcut + ">");
					writer.Flush();
					string? studentUsername = reader.ReadLine();
					Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/grades/show/" + subjectShortcut + "/" + classShortcut + ">" + studentUsername);

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
						writer.WriteLine("\n\r==== Zobrazení známek studenta ====");
						grades.ShowGrades(subjectShortcut, studentUsername);
						writer.WriteLine("\n\r===================================");
						writer.Flush();
						return;
					}
				}
			}
		}
	}
}
