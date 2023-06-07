using System.Data.SqlClient;

namespace Projekt_PV
{
	internal class CheckClass
	{
		/// <summary>
		/// Tato třída slouží pro kontrolu, zda třída je zapsána v databází (pokud tedy existuje):
		/// </summary>
		string connectionString;
		string? className;
		public CheckClass(string connectionString, string? className)
		{
			this.connectionString = connectionString;
			this.className = className;
		}
		/// <summary>
		/// Metoda, která kontroluje, zda třída je zapsána v databází:
		/// </summary>
		/// <returns></returns>
		public bool CheckClassInDatabase()
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM trida WHERE nazev = @ClassName", connection);
				command.Parameters.AddWithValue("@ClassName", className);
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
