using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class Server : MonoBehaviour {

	[Header("Connection")]
	public int serverPort;
	public int clientPort;

	public float fakeDelay;
	public float fakePacketLoss;
	public float snapshotSendRate;
	private Channel channel;
	private float lastSnapshotSentTime;

	[Header("Game")]
	public Object playerPrefab;
	private List<ServerPlayerController> players = new List<ServerPlayerController>();

	private static Server instance = null;
	public static Server Instance {
		get {
			return instance;
		}	
	}

    private const string PLAYER = "Player";

    void Awake() {
		instance = this;
	}

	void Start() {
		channel = new Channel(null, serverPort, clientPort);
		lastSnapshotSentTime = Time.realtimeSinceStartup;
	}

	void OnDestroy() {
		channel.Disconnect();
		instance = null;
	}

	void Update() {
		Packet inPacket = channel.GetPacket ();
		if (inPacket != null) {
			BitBuffer bitBuffer = inPacket.buffer;
			int messageCount = bitBuffer.GetInt ();
			for (int i = 0; i < messageCount; i++) {
				Message clientMessage = ReadClientMessage(bitBuffer, inPacket.fromEndPoint);
                if (clientMessage != null) {
                    switch (clientMessage.Type) {
                        case MessageType.CONNECT_PLAYER:
                            ProcessConnectPlayer(clientMessage as ConnectPlayerMessage); break;
                        case MessageType.DISCONNECT_PLAYER:
                            ProcessDisconnectPlayer(clientMessage as DisconnectPlayerMessage); break;
                        default:
                            ServerPlayerController player = GetPlayerWithEndPoint(inPacket.fromEndPoint);
                            if (player != null) {
                                player.CommunicationManager.ReceiveMessage(clientMessage);
                            }
                            break;
                    }
                }
			}
		}

        SendSnapshot();
        SendPackets();
	}

    private void SendSnapshot() {
        float currentTime = Time.realtimeSinceStartup;
        float ttsSnapshot = 1.0f / snapshotSendRate;
        if (currentTime - lastSnapshotSentTime >= ttsSnapshot) {
            SnapshotMessage snapshotMessage = new SnapshotMessage(-1, BuildGameData()) {
                TimeToSend = fakeDelay
            };
            for (int i = 0; i < players.Count; i++) {
                players[i].CommunicationManager.SendMessage(snapshotMessage);
            }
            lastSnapshotSentTime = currentTime;
        }
    }

    private void SendPackets() {
        for (int playerIdx = 0; playerIdx < players.Count; playerIdx++) {
            ServerPlayerController player = players[playerIdx];
            Packet packet = player.CommunicationManager.BuildPacket();

            if (packet != null) {
                bool shouldDropPacket = Random.Range(0.0001f, 100.0f) < fakePacketLoss;
                if (!shouldDropPacket) {
                    channel.Send(packet, player.endPoint);
                }
            }
        }
    }

    Message ReadClientMessage(BitBuffer bitBuffer, IPEndPoint clientEndPoint) {
		MessageType messageType = bitBuffer.GetEnum<MessageType> ((int) MessageType.TOTAL);
		Message clientMessage = null;

		switch (messageType) {
		    case MessageType.CONNECT_PLAYER:
			    clientMessage = ConnectPlayerMessage.CreateConnectPlayerMessageToReceive(clientEndPoint); break;
		    case MessageType.DISCONNECT_PLAYER:							
			    clientMessage = new DisconnectPlayerMessage(); break;
		    case MessageType.PLAYER_INPUT:
			    clientMessage = new PlayerInputMessage(); break;
		    case MessageType.ACK_RELIABLE_MAX_WAIT_TIME:
                clientMessage = AckReliableMessage.CreateAckReliableMessageToReceive(); break;
		    case MessageType.ACK_RELIABLE_SEND_EVERY_PACKET:
			    clientMessage = AckReliableSendEveryFrameMessage.CreateAckReliableSEFMToReceive(); break;
		    default:
			    Debug.LogError("Unknown client message received.");
			    return null;
		}
		clientMessage.From = clientEndPoint;
		clientMessage.Load(bitBuffer);

		return clientMessage;
	}

	public void ProcessConnectPlayer(ConnectPlayerMessage connectPlayerMessage) {
		int playerId = connectPlayerMessage.PlayerId;
		ServerPlayerController player = GetPlayerWithId(playerId);
		if (player != null) {
			DisconnectPlayer(player);
		}

		GameObject playerGO = Instantiate (playerPrefab) as GameObject;
		playerGO.name = PLAYER + " " + playerId; 
		player = playerGO.GetComponent<ServerPlayerController>();
		player.endPoint = connectPlayerMessage.EndPoint;
		player.Id = playerId;
		players.Add(player);

        BroadcastConnection(playerId);
	}	

    private void BroadcastConnection(int playerId) {
        for (int i = 0; i < players.Count; i++) {
            ServerPlayerController playerToSendTo = players[i];
            PlayerConnectedMessage playerConnectedMessage = PlayerConnectedMessage.CreatePCMToSend(playerToSendTo, playerId);
            playerToSendTo.CommunicationManager.SendMessage(playerConnectedMessage);
        }
    }

	void ProcessDisconnectPlayer(DisconnectPlayerMessage disconnectPlayerMessage) {
		ServerPlayerController player = GetPlayerWithEndPoint(disconnectPlayerMessage.From);
		if (player != null) {
			DisconnectPlayer(player);
			for (int i = 0; i < players.Count; i++) {
				ServerPlayerController playerToSendTo = players[i];
				PlayerDisconnectedMessage playerDisconnectedMessage = PlayerDisconnectedMessage.CreatePDMToSend(playerToSendTo, player.Id);
				playerToSendTo.CommunicationManager.SendMessage(playerDisconnectedMessage);
			}
		}					
	}

	void DisconnectPlayer(ServerPlayerController player) {
		Destroy(player.gameObject);
		players.Remove(player);
	}

	ServerPlayerController GetPlayerWithId(int playerId) {
		for (int i = 0; i < players.Count; i++) {
			if (playerId == players[i].Id) {
				return players[i];
			}
		}
		return null;
	}

	public ServerPlayerController GetPlayerWithEndPoint(IPEndPoint endPoint) {
		for (int i = 0; i < players.Count; i++) {
			if (players[i].endPoint.Equals(endPoint)) {
				return players[i];
			}
		}
		return null;
	}

	private GameData BuildGameData() {
        GameData gameData = new GameData {
            Time = Time.realtimeSinceStartup
        };
        for (int i = 0; i < players.Count; i++) {
			gameData.Players.Add(players[i].BuildPlayerData());
		}
		return gameData;
	}
}
