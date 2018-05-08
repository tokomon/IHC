using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;

using UnityEngine;

namespace AssemblyCSharp
{
	[Serializable]
	public class PlayerInfo
	{
		public const int CLIENT_TYPE_PLAYER					= 1;
		public const int CLIENT_TYPE_SPECTATOR				= 2;

		public const int OPTION_CONNECTION					= 1;
		public const int OPTION_LOGIN						= 2;
		public const int OPTION_WAIT_PLAYERS				= 3;
		public const int OPTION_SET_GAME					= 4;
		public const int OPTION_SET_MAP						= 5;
		public const int OPTION_PLAYER_POSITION				= 6;
		public const int OPTION_POSITION_VALIDATION			= 7;
		public const int OPTION_START						= 8;
		public const int OPTION_UPDATE_PLAYER_POSITION		= 9;
		public const int OPTION_FREE_SPACE					= 10;
		public const int OPTION_SPACES						= 11;
		public const int OPTION_NOT_VALID					= 12;
		//		public const int OPTION_SEND_NEW_MAP				= 13;
		//		public const int OPTION_READY						= 14;
		//		public const int OPTION_START_AGAIN					= 15;
		public const int OPTION_GAME_FINISHED				= 20;
		public const int OPTION_DISCONNECT					= 99;

		public Socket socket;
		public int sendPositionOption = 0;
		private byte[] _recieveBuffer = new byte[8192];
		DateTime time = DateTime.Now;

		public int option;
		public int player_id;

		public int counter=20;

		public int matrix_size;
		public String map_data;

		public int matrix_free_x;
		public int matrix_free_y;

		public int liberar_x;
		public int liberar_y;

		public int player_type;
		public int player_winner;
		public bool winner=false;
		public bool read_winner=false;
		public String message;

		public bool alertar_jugador = false;
		public bool actualizar_map = false;
		public bool alertar_forzado = false;

		private static PlayerInfo instance = null;

		public PlayerInfo (){
		}
		public static PlayerInfo Instance
		{
			get
			{
				if (instance == null)
					instance = new PlayerInfo();
				return instance;
			}
		}
		public bool startConnection(String ip, int port,int type){
			try {
				read_winner=false;
				socket = new Socket (
					AddressFamily.InterNetwork,
					SocketType.Stream,
					ProtocolType.Tcp
				);
				socket.Connect (new IPEndPoint (IPAddress.Parse(ip), port));
				player_type = type;
				socket.BeginReceive (
					_recieveBuffer,
					0,
					_recieveBuffer.Length,
					SocketFlags.None,
					new AsyncCallback (ReceiveCallback),
					null
				);
			} catch (SocketException ex) {
				Debug.Log (ex.Message);
				socket = null;
				return false;
			}
			return true;
		}
		public void endConnection(){
			sendPositionOption = 0;
			socket.Disconnect (false);
			socket = null;
			read_winner = false;
			alertar_forzado = false;
			actualizar_map = false;
			alertar_jugador = false;
			winner = false;
		}
		void sendString(Socket socket,String message){
			try {
				//byte[] a1 = BitConverter.GetBytes(message.Length);
				byte[] a2 = System.Text.Encoding.ASCII.GetBytes(message);
				//byte[] message_bytes = new byte[a1.Length + a2.Length];
				//System.Buffer.BlockCopy(a1,0,message_bytes,0,a1.Length);
				//System.Buffer.BlockCopy(a2,0,message_bytes,a1.Length,a2.Length);
				//int bytes = socket.Send(message_bytes);
				int bytes = socket.Send(a2);
				Debug.LogFormat ("Sending: {0} bytes", bytes);
				Debug.LogFormat ("Sending: {0}", message.ToString());
			}
			catch (SocketException ex) {
				Debug.Log (ex.Message);
			}
		}
		public void workData(){
			switch(option)
			{
			case OPTION_CONNECTION: // PLAYER and SPECTATOR
				{
					sendLogin ();
					break;
				}
			case OPTION_WAIT_PLAYERS: // PLAYER and SPECTATOR
				{
					waitPlayers();
					break;
				}
			case OPTION_SET_GAME: // PLAYER
				{
					setGame();
					break;
				}
			case OPTION_SET_MAP: // PLAYER
				{
					iniMap();
					break;
				}
			case OPTION_POSITION_VALIDATION: // PLAYER
				{
					validated();
					break;
				}
			case OPTION_START: // PLAYER
				{
					startGame();
					break;
				}
			case OPTION_SPACES: // PLAYER
				{
					while (actualizar_map) {
					};
					if (socket != null) {
						liberar_x = matrix_free_x;
						liberar_y = matrix_free_y;
						actualizar_map = true;
					}
					break;
				}
			case OPTION_NOT_VALID: // PLAYER
				{
					alertar_jugador = true;
					break;
				}/*
			case OPTION_SEND_NEW_MAP: // PLAYER
				{
					reloadMap ();
					sendReady ();
					break;
				}
			case OPTION_START_AGAIN: // PLAYER
				{
					startGameAgain();
					break;
				}*/
			case OPTION_DISCONNECT:
				{
					alertar_forzado = true;
					break;
				}
			case OPTION_GAME_FINISHED:
				{
					winner = player_winner != player_id;
					read_winner = true;
					break;
				}
			}
		}
		// Funciones Importantes
		void waitPlayers(){
			// Esperando Jugadores
		}
		void setGame(){
			// Mostrar ID
			// Esperando Mapa
		}
		void iniMap(){
			Debug.LogFormat ("Size: {0}", matrix_size.ToString());
			Debug.LogFormat ("Map: {0}", map_data.ToString());
			// Cargar Mapa
			sendPositionOption = 1;
		}
		void validated(){
			sendPositionOption = 0;
			// Esperando Jugadores
		}
		void startGame(){
			sendPositionOption = 2;
			// 3 2 1 GO
		}
		public void sendFreeSpace(int free_x,int free_y){
			PacketFree free = new PacketFree ();
			free.option= OPTION_FREE_SPACE;
			free.matrix_free_x = free_x;
			free.matrix_free_y = free_y;
			free.player_id = player_id;
			sendString(socket,JsonUtility.ToJson(free));
		}/*
		public void mineGame(){
			// Mostrar Perdio y Esperando Nuevo Mapa
			sendPositionOption = 3;
		}
		void reloadMap(){
			sendPositionOption = 0;
			// Cargar Nuevo Mapa
		}
		void startGameAgain(){
			sendPositionOption = 2;
			// 3 2 1 GO
		}*/
		// Funciones Importantes
		void sendLogin(){
			PacketLogin login = new PacketLogin ();
			login.option = OPTION_LOGIN;
			login.type = player_type;
			sendString(socket,JsonUtility.ToJson(login));
		}
		public void sendPosition(){
			TimeSpan elapsed = DateTime.Now - time;
			if (sendPositionOption > 0 && elapsed.TotalMilliseconds > 1000) {
				time = DateTime.Now;
				PacketPosition position = new PacketPosition ();
				switch (sendPositionOption) {
				case 1:
					position.option = OPTION_PLAYER_POSITION;
					counter = counter - 1;
					break;
				case 2:
					position.option = OPTION_UPDATE_PLAYER_POSITION;
					break;/*
				case 3:
					position.option = OPTION_NEW_MAP;
					break;*/
				}
				position.player_id = player_id;
				position.matrix_pos_x = counter;
				position.matrix_pos_y = 0;
				sendString (socket, JsonUtility.ToJson (position));
			}
		}/*
		void sendReady(){
			PacketSimple ready = new PacketSimple ();
			ready.option= OPTION_READY;
			ready.player_id = player_id;
			sendString(socket,JsonUtility.ToJson(ready));
		}*/
		// RECV
		private void ReceiveCallback(IAsyncResult AR){
			//Check how much bytes are recieved and call EndRecieve to finalize handshake
			int recieved = socket.EndReceive(AR);
			if (recieved <= 0) {
				return;
			}
			//Copy the recieved data into new buffer , to avoid null bytes
			byte[] recData = new byte[recieved];
			Buffer.BlockCopy(_recieveBuffer,0,recData,0,recieved);
			// Saving data in playerInfo
			receiveString(recData);
			//workData
			workData();
			if (option != OPTION_GAME_FINISHED && option != OPTION_DISCONNECT) {
				socket.BeginReceive (
					_recieveBuffer,
					0,
					_recieveBuffer.Length,
					SocketFlags.None,
					new AsyncCallback (ReceiveCallback),
					null
				);
			}
		}
		void receiveString(byte[] recData){
			Debug.Log ("Me llego algo wey");
			String str_size = System.Text.Encoding.ASCII.GetString (recData,0,4);
			int json_size;
			Int32.TryParse (str_size, out json_size);
			Debug.LogFormat ("Json Size: {0}", json_size);
			String json_str = System.Text.Encoding.ASCII.GetString (recData,4,json_size);
			Debug.LogFormat ("Data: {0}", json_str.ToString());
			JsonUtility.FromJsonOverwrite(json_str.ToString(), this);
			Debug.LogFormat ("Recived Option: {0}", option);
		}
	}
}

