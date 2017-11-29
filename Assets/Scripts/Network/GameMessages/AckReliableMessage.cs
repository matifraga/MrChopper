public class AckReliableMessage : Message {

	private int idToAck;

	public static AckReliableMessage CreateAckReliableMessageToSend(int idToAck) {
		return new AckReliableMessage(-1, idToAck);
	}

	public static AckReliableMessage CreateAckReliableMessageToReceive() {
		return new AckReliableMessage(-1, -1);
	}
		
	private AckReliableMessage(int id, int _idToAck) : base(id, MessageType.ACK_RELIABLE_MAX_WAIT_TIME, ReliabilityType.UNRELIABLE) {		
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
