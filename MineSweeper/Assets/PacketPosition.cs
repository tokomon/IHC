using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class PacketPosition
	{
		public int player_id;
		public int option;
		public int matrix_pos_x;
		public int matrix_pos_y;
		public PacketPosition ()
		{
		}
	}
}

