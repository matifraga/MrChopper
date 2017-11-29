public class AckReliableSendEveryFrameMessage : Message {

	private int idToAck;

	public static AckReliableSendEveryFrameMessage CreateAckReliableSEFMToSend(int idToAck) {
		return new AckReliableSendEveryFrameMessage(-1, idToAck);
	}

	public static AckReliableSendEveryFrameMessage CreateAckReliableSEFMToReceive() {
		return new AckReliableSendEveryFrameMessage(-1, -1);
	}

	private AckReliableSendEveryFrameMessage(int id, int _idToAck) : base(id, MessageType.ACK_RELIABLE_SEND_EVERY_PACKET, ReliabilityType.UNRELIABLE) {
		idToAck = _idToAck;
	}

	public override void Load(BitBuffer bitBuffer) {
		base.Load(bitBuffer);
		idToAck = bitBuffer.GetInt();
	}

	public override void Save(BitBuffer bitBuffer) {
		base.Save(bitBuffer);
		bitBuffer.PutInt(idToAck);
	}

	public int IdToAck {
		get {
			return idToAck;
		}
	}

}
