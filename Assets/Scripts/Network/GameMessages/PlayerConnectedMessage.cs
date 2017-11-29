public class PlayerConnectedMessage : Message {
	private int playerId;

	public static PlayerConnectedMessage CreatePCMToSend(ServerPlayerController receiver, int playerConnectedId) {
		int messageId = receiver.CommunicationManager.GetReliableSEFMessageId();
		return new PlayerConnectedMessage(messageId, playerConnectedId);
	}

	public static PlayerConnectedMessage CreatePCMToReceive() {
		return new PlayerConnectedMessage(-1, -1);
	}
		
	private PlayerConnectedMessage(int id, int _playerId) : base(id, MessageType.PLAYER_CONNECTED, ReliabilityType.RELIABLE_SEND_EVERY_PACKET) {
		playerId = _playerId;
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
