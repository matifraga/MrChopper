public class DisconnectPlayerMessage : Message {

	public DisconnectPlayerMessage() : base(-1, MessageType.DISCONNECT_PLAYER, ReliabilityType.RELIABLE_MAX_WAIT_TIME) { }

	public DisconnectPlayerMessage(int id, int playerId) : base(id, MessageType.DISCONNECT_PLAYER, ReliabilityType.RELIABLE_MAX_WAIT_TIME) { }		
}

