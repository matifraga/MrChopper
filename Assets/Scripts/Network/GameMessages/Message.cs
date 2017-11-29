using System.Net;

public class Message {
	private MessageType messageType;
	private ReliabilityType reliabilityType;
    private float reliableMaxTime;

    private int id;
	private float timeToSend;

    private IPEndPoint fromEndPoint;

	public Message(int _id, MessageType _messageType, ReliabilityType _reliabilityType) {
		messageType = _messageType;
		id = _id;
		reliabilityType = _reliabilityType;
	}

	public void Update(float dt) {
		timeToSend -= dt;
	}	

	public virtual void Load(BitBuffer bitBuffer) {
		id = bitBuffer.GetInt();
	}

	public virtual void Save(BitBuffer bitBuffer) {
		bitBuffer.PutEnum(messageType, (int) MessageType.TOTAL);
		bitBuffer.PutInt(id);
	}		
		
	public MessageType Type {
		get {
			return messageType;
		}
	}

	public ReliabilityType Reliability {
		get {
			return reliabilityType;
		}
	}

	public bool NeedsToBeSent {
		get {
			return timeToSend <= 0;
		}
	}

	public float TimeToSend {
		set {
			timeToSend = value;
		}
	}

	public float ReliableMaxTime {
		get {
			return reliableMaxTime;
		}
		set {
			reliableMaxTime = value;
		}
	}	

	public int ReliabilityId {
		get {
			return id;
		}
	}

	public IPEndPoint From {
		get {
			return fromEndPoint;
		}
		set {
			fromEndPoint = value;
		}
	}
}
	


