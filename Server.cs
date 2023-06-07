using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Projekt_PV
{
	internal class Server
	{
		private TcpListener myServer;
		private bool isRunning;
		private string IP;
		private int port;
		/// <summary>
		/// Vytvoří server na dané IP a portu
		/// </summary>
		/// <param name="IP">string</param>
		/// <param name="port">int</param>
		public Server(string IP, int port)
		{
			this.IP = IP;
			this.port = port;
			myServer = new TcpListener(IPAddress.Parse(IP), port);
			myServer.Start();
			isRunning = true;
			ServerLoop();
		}
		/// <summary>
		/// Zapnutí samotné funkcionality serveru (Ve smyčce)
		/// </summary>
		private void ServerLoop()
		{
			Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "Server byl spusten na adrese " + IP + ":" + port.ToString());
			while (isRunning)
			{
				TcpClient client = myServer.AcceptTcpClient();

				ClientLoop(client);
			}
		}
		/// <summary>
		/// Zapnutí samotné funkcionality serveru na straně clienta
		/// </summary>
		/// <param name="client">TcpClient</param>
		private void ClientLoop(TcpClient client)
		{
			StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8);
			StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8);

			writer.WriteLine("Byl/a jsi pripojen/a k serveru.");
			writer.Flush();
			Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + "Klient připojen");
			bool clientConnect = true;
			Login login = new Login(writer, reader);
			Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + "Přihlášen/á : " + login.GetFirstName() + " " + login.GetLastName());
			string? data = null;
			string? dataRecive = null;
			bool newLogin = false;
			while (clientConnect)
			{
				if (newLogin)
				{
				}
				writer.Write(login.GetUsername() + ">");
				writer.Flush();
				data = reader.ReadLine();
				data = data.ToLower();
				Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + login.GetUsername() + ">" + data);
				if (data == "exit")
				{
					clientConnect = false;
					Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + login.GetFirstName() + " " + login.GetLastName() + " odpojen/a.");
					writer.WriteLine("Byl/a jsi odpojen/a");
					writer.Flush();
					client.Close();
					break;
				}
				else
				{
					ClientHandler clientHandler = new ClientHandler(data, login.GetIsTeacher(), login.GetConnectionString(), writer, reader, login.GetUsername());
					dataRecive = clientHandler.Response();
				}
				if (dataRecive == "logout")
				{
					Console.WriteLine("\n\r[" + DateTime.Now.ToString() + "] " + "Odhlášen/á : " + login.GetFirstName() + " " + login.GetLastName());
					writer.WriteLine("\n\r===========================\n\rOdhlášení proběhlo úspěšně. \n\r===========================\n\r");
					writer.Flush();
					login.Logout();
					Console.Write("\n\r[" + DateTime.Now.ToString() + "] " + "Přihlášen/á : " + login.GetFirstName() + " " + login.GetLastName());
				}
				if (dataRecive != "logout")
				{
					writer.WriteLine(dataRecive);
					writer.Flush();
				}
			}
		}
	}
}