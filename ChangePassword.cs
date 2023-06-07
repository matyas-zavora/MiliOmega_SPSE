using System.Data.SqlClient;

namespace Projekt_PV
{
	internal class ChangePassword
	{
		/// <summary>
		/// Změna hesla uživatele.
		/// </summary>
		/// <param name="writer">StreamWriter</param>
		/// <param name="reader">StreamReader</param>
		/// <param name="connectionString">string</param>
		/// <param name="username">string</param>
		public ChangePassword(StreamWriter writer, StreamReader reader, string connectionString, string username)
		{
			do
			{
				writer.WriteLine("\n\rZadejte nové heslo ('cancel' pro návrat) :");
				writer.Write(username + "/password>");
				writer.Flush();
				string? newPassword = reader.ReadLine();
				Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/password>" + newPassword);

				if (newPassword == null)
				{
					writer.WriteLine("\n\rZadejte prosím heslo. Zkuste to znovu.");
					writer.Flush();
					continue;
				}
				else if (newPassword == "cancel")
				{
					return;
				}
				else
				{
					writer.WriteLine("\n\rZadejte nové heslo znovu:");
					writer.Write(username + "/password/" + newPassword + ">");
					writer.Flush();
					string? newPasswordAgain = reader.ReadLine();
					Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + username + "/password/" + newPassword + ">" + newPasswordAgain);

					if (newPasswordAgain == null)
					{
						writer.WriteLine("\n\rZadejte prosím heslo znovu. Zkuste to znovu.");
						writer.Flush();
						continue;
					}
					else if (newPasswordAgain == "cancel")
					{
						return;
					}
					else if (newPassword == newPasswordAgain)
					{
						using (SqlConnection connection = new SqlConnection(connectionString))
						{
							connection.Open();
							SqlCommand command = new SqlCommand("UPDATE uzivatele SET _password = @Password WHERE username = @Username", connection);
							command.Parameters.AddWithValue("@Password", newPassword);
							command.Parameters.AddWithValue("@Username", username);
							command.ExecuteNonQuery();
							writer.WriteLine("\n\rHeslo bylo úspěšně změněno.");
							writer.Flush();
							break;
						}
					}
					else
					{
						writer.WriteLine("\n\rHesla se neshodují. Zkuste to znovu.");
						writer.Flush();
					}
				}
			} while (true);
		}
	}
}
