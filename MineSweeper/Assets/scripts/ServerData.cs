﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;

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
		public const int OPTION_MAPS_SPECTATOR				= 30;
		public const int OPTION_SPECTATOR_JUGADAS			= 31;
		public const int OPTION_DISCONNECT					= 99;

		public Socket socket;
		public int sendPositionOption = 0;
		private byte[] _recieveBuffer = new byte[256];
		private String actualBuffer="";
		private int json_size_data=-1;
		DateTime time = DateTime.Now;

		public int option;
		public int player_id;

		public int counter=5;

		public int matrix_size;
		public String map_data;

		public int matrix_free_x;
		public int matrix_free_y;

		public int pos_x;
		public int pos_y;

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
		public bool read_map = false;

		private static PlayerInfo instance = null;

		private bool flagClient = false;
		private bool flagQueue = false;
		private int turno = 0;

		public String maps_data;
		public String jugadas;
		public Queue<Vector3> listaJugadas;
		public PlayerInfo (){
			listaJugadas = new Queue<Vector3> ();
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
				flagClient = false;
				flagQueue = false;
				turno = 0;
				read_map=false;
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
		private void canClientReadQueue(){
			flagClient = true;
			turno = 0;
			while(flagQueue && turno == 0)
				;
		}
		private void freeClientReadQueue(){
			flagClient = false;
		}
		private void canQueueReadQueue(){
			flagQueue = true;
			turno = 1;
			while(flagClient && turno == 1)
				;
		}
		private void freeQueueReadQueue(){
			flagQueue = false;
		}
		private void addJugada(){
			canQueueReadQueue();
			listaJugadas.Enqueue (new Vector3 (liberar_x, liberar_y, player_id));
			freeQueueReadQueue();
		}
		private void readJugadas (){
			canQueueReadQueue();
			try{
				string[] datos = jugadas.Split('&');
				for (int i = 0; i < datos.Length; i += 3) {
					if(i + 2 < datos.Length){
						int player_id = 0;
						int x;
						int y;
						bool value = Int32.TryParse (datos [i], out player_id);
						value &= Int32.TryParse (datos [i+1], out x);
						value &= Int32.TryParse (datos [i+2], out y);
						if(value)
							listaJugadas.Enqueue (new Vector3 (x, y, player_id));
					}
				}
			}
			catch(Exception ex){
				Debug.Log (ex.Message);
			};
			freeQueueReadQueue();
		}
		public bool hayJugada()
		{
			return listaJugadas.Count > 0;
		}
		public Vector3 get_jugada(){
			canClientReadQueue ();
			Vector3 returnValue = listaJugadas.Dequeue ();
			freeClientReadQueue ();
			return returnValue;
		}
		public void endConnection(){
			flagClient = false;
			flagQueue = false;
			read_map=false;
			sendPositionOption = 0;
			int i=0;
			sendDisconnect();
			while(i<1000000){
				++i;
			}
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
				}
			case OPTION_MAPS_SPECTATOR: // SPECTATOR
				{
					Debug.LogFormat ("Size: {0}", matrix_size.ToString());
					Debug.LogFormat ("Map: {0}", maps_data.ToString());
					Debug.LogFormat ("Jugadas: {0}", jugadas.ToString());
					int total_players = maps_data.Split('&').Length;
					for (int i = 0; i < total_players; ++i) {
						liberar_x = 0;
						liberar_y = 0;
						player_id = i + 1;
						addJugada ();
					}
					readJugadas ();
					read_map = true;
					break;
				}
			case OPTION_SPECTATOR_JUGADAS: // SPECTATOR
				{
					addJugada ();
					break;
				}
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
			pos_x = 0;
			pos_y = 0;
			// Mostrar ID
			// Esperando Mapa
		}
		void iniMap(){
			Debug.LogFormat ("Size: {0}", matrix_size.ToString());
			Debug.LogFormat ("Map: {0}", map_data.ToString());
			read_map=true;
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
					break;
				case 2:
					position.option = OPTION_UPDATE_PLAYER_POSITION;
					break;/*
				case 3:
					position.option = OPTION_NEW_MAP;
					break;*/
				}
				position.player_id = player_id;
				position.matrix_pos_x = pos_x;
				position.matrix_pos_y = pos_y;
				sendString (socket, JsonUtility.ToJson (position));
			}
		}
		void sendDisconnect(){
			PacketSimple ready = new PacketSimple ();
			ready.option= OPTION_DISCONNECT;
			sendString(socket,JsonUtility.ToJson(ready));
		}
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
			if (receiveString (recData,recieved)) {
				//workData
				workData ();
			}
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
		bool receiveString(byte[] recData, int recieved){
			bool return_value = false;
			Debug.LogFormat ("Package Size: {0}", recieved);
			actualBuffer += System.Text.Encoding.ASCII.GetString (recData, 0, recieved);
			if (json_size_data < 0 && actualBuffer.Length >= 6) {
				Debug.Log ("Inicio de un Paquete");
				String str_size = actualBuffer.Substring (0, 6);
				Int32.TryParse (str_size, out json_size_data);
				actualBuffer = actualBuffer.Substring (6, actualBuffer.Length - 6);
				Debug.LogFormat ("Json Size: {0}", json_size_data);
			}
			if (actualBuffer.Length >= json_size_data) {
				String json_str = actualBuffer.Substring(0,json_size_data);
				actualBuffer = actualBuffer.Substring (json_size_data, actualBuffer.Length - json_size_data);
				Debug.LogFormat ("Data: {0}", json_str.ToString());
				try{
					JsonUtility.FromJsonOverwrite(json_str.ToString(), this);
					Debug.LogFormat ("Recived Option: {0}", option);
					return_value = true;
				} catch (Exception ex) {
					Debug.Log (ex.Message);
				}
				json_size_data = -1;
				Debug.Log ("Fin de un Paquete");
			}
			return return_value;
		}
	}
}

