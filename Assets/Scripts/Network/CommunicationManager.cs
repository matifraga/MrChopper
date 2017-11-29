using System.Collections.Generic;
using UnityEngine;

public class CommunicationManager {

	private List<Message> outMessages = new List<Message>();
	private int messagesCount = 0;
	private List<Message> inMessages = new List<Message>();

	private int latestMessageId = -1;
	private int latestSEFMessageId = -1;
	private int reliableSEFMessageId = 0;
	private int reliableMessageId = 0;

	private void Update() {
		messagesCount = 0;
		for (int i = 0; i < outMessages.Count; i++) {			
			Message message = outMessages [i];
			message.Update(Time.deltaTime);
			if (message.NeedsToBeSent) {
				messagesCount++;
			}
		}
	}

	public void SendMessage(Message message) {
		outMessages.Add(message);
	}

	public void ReceiveMessage(Message message) {
		switch (message.Reliability) {
			case ReliabilityType.RELIABLE_MAX_WAIT_TIME:
				SendMessage(AckReliableMessage.CreateAckReliableMessageToSend(message.ReliabilityId));
				if (message.ReliabilityId != (LatestReliableMessageId + 1)) {
					return;	
				} 
				LatestReliableMessageId = message.ReliabilityId;
				SendMessage(AckReliableMessage.CreateAckReliableMessageToSend(message.ReliabilityId));
				break;
			case ReliabilityType.RELIABLE_SEND_EVERY_PACKET:
				SendMessage(AckReliableSendEveryFrameMessage.CreateAckReliableSEFMToSend(message.ReliabilityId));
				if (message.ReliabilityId <= LatestSEFMessageIdReceived) {	
					return;	
				} 
				LatestSEFMessageIdReceived = message.ReliabilityId;
				break;
		}

		switch (message.Type) {
			case MessageType.ACK_RELIABLE_MAX_WAIT_TIME:			
				ProcessAckReliable(message as AckReliableMessage); break;
			case MessageType.ACK_RELIABLE_SEND_EVERY_PACKET:
				ProcessAckReliableSEFM(message as AckReliableSendEveryFrameMessage); break;
			default:
				inMessages.Add(message);
				break;	
			}
	}

	private void ProcessAckReliable(AckReliableMessage ackReliableMessage) {		
		for (int i = 0; i < outMessages.Count; i++) {
			if (outMessages[i].ReliabilityId == ackReliableMessage.IdToAck) {
				outMessages.RemoveAt(i);
				break;
			}
		}
	}

	private void ProcessAckReliableSEFM(AckReliableSendEveryFrameMessage ackReliableSEFM) {
		for (int i = 0; i < outMessages.Count; i++) {
			if (outMessages[i].Reliability == ReliabilityType.RELIABLE_SEND_EVERY_PACKET &&
				outMessages[i].ReliabilityId <= ackReliableSEFM.IdToAck) {
				outMessages.RemoveAt(i);
				i--;
			}
		}
	}

	public Packet BuildPacket() {
		Update();
		if (messagesCount > 0) {
			Packet outPacket = new Packet();
			outPacket.buffer.PutInt(messagesCount);
			for (int i = 0; i < outMessages.Count; i++) {
				Message serverMessage = outMessages[i];
				if (serverMessage.NeedsToBeSent) {
					serverMessage.Save(outPacket.buffer);
					if (serverMessage.Reliability == ReliabilityType.UNRELIABLE) {
						outMessages.RemoveAt(i);
						i--;
					} else {
						serverMessage.TimeToSend = serverMessage.Reliability == ReliabilityType.RELIABLE_SEND_EVERY_PACKET ? 0 : serverMessage.ReliableMaxTime;
					}
				}
			}
			outPacket.buffer.Flip();
			return outPacket;
		} else {
			return null;
		}
	}

	public Message GetMessage() {
		if (inMessages.Count > 0) {
			Message message = inMessages[0];
			inMessages.RemoveAt(0);
			return message;
		} 
		return null;
	}

	public bool HasMessage() {
		return inMessages.Count > 0;
	}

	public int GetReliableSEFMessageId() {
		return ++reliableSEFMessageId;
	}

	public int GetReliableMessageId() {
		return ++reliableMessageId;
	}

	public int ReliableSEFMessageId {
		get {
			return reliableSEFMessageId;
		}
	}

	public int ReliableMessageId {
		get {
			return reliableMessageId;
		}
	}

	public int LatestReliableMessageId {
		get {
			return latestMessageId;
		}
		set {
			latestMessageId = value;
		}
	}

	public int LatestSEFMessageIdReceived {
		get {
			return latestSEFMessageId;
		}
		set {
			latestSEFMessageId = value;
		}
	}
}
