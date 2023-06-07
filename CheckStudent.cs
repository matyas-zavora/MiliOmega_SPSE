using System.Data.SqlClient;

namespace Projekt_PV
{
	internal class CheckStudent
	{
		/// <summary>
		/// Třída pro kontrolu, zda student existuje v databázi
		/// </summary>
		string connectionString;
		string? studentUsername;
		public CheckStudent(string connectionString, string? studentUsername)
		{
			this.connectionString = connectionString;
			this.studentUsername = studentUsername;
		}
		/// <summary>
		/// Metoda pro kontrolu, zda student existuje v databázi
		/// </summary>
		/// <returns></returns>
		public bool CheckStudentInDatabase()
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM uzivatele WHERE username = @Username", connection);
				command.Parameters.AddWithValue("@Username", studentUsername);
				int count = (int)command.ExecuteScalar();
				if (count == 0)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}
	}
}
