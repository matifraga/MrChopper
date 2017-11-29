public class PlayerDisconnectedMessage : Message {
	private int playerId;

	public static PlayerDisconnectedMessage CreatePDMToSend(ServerPlayerController receiver, int playerDisconnectedId) {
		int messageId = receiver.CommunicationManager.GetReliableSEFMessageId();
		return new PlayerDisconnectedMessage(messageId, playerDisconnectedId);
	}

	public static PlayerDisconnectedMessage CreatePDMToReceive() {
		return new PlayerDisconnectedMessage(-1, -1);
	}

	private PlayerDisconnectedMessage(int id, int playerId) : base(id, MessageType.PLAYER_DISCONNECTED, ReliabilityType.RELIABLE_SEND_EVERY_PACKET) {
	}

	public override void Load(BitBuffer bitBuffer) {
		base.Load(bitBuffer);
		playerId = bitBuffer.GetInt();
	}

	public override void Save(BitBuffer bitBuffer) {
		base.Save(bitBuffer);
		bitBuffer.PutInt(playerId);
	}

	public int PlayerId {
		get {
			return playerId;
		}
	}
}
