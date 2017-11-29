using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Client : MonoBehaviour {

	public int serverPort;
	public int clientPort;
	private Channel channel;
	private bool isConnected = false;
	private CommunicationManager communicationManager = new CommunicationManager();

	private List<PlayerNetworkView> players = new List<PlayerNetworkView>();
	public int playerId;
    public GameObject playerPrefab;
    private ClientPlayerController ownPlayer;

	public int buffDesiredLength;
	public double maxDiffTime;
	public double simSpeed;
	public double frameRate;
	private double simIniTime;
	private double simTime;
	private List<GameData> snapshots = new List<GameData>();

    private const string PLAYER = "Player";
    private const string LOCALHOST = "127.0.0.1";

    void Start() {
		channel = new Channel(LOCALHOST, clientPort, serverPort);
	}

	void OnDestroy() {
		channel.Disconnect();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space) && !isConnected) {
			communicationManager.SendMessage(ConnectPlayerMessage.CreateConnectPlayerMessageToSend(playerId));
		} else if (Input.GetKeyDown(KeyCode.Escape) && isConnected) {
			communicationManager.SendMessage(new DisconnectPlayerMessage(Random.Range(0, int.MaxValue), playerId));
            Clean();
		}

		ReadMessages();	

		if (simIniTime != 0) {
			if (!Input.GetKey(KeyCode.K)) {
				Interpolate();
			}
		}

		if (ownPlayer != null) {
			communicationManager.SendMessage(new PlayerInputMessage(playerId, ownPlayer.playerInput));
		}

		Packet outPacket = communicationManager.BuildPacket();
		if (outPacket != null) {
			channel.Send(outPacket);
		}
	}

    private void Clean() {
        DisconnectPlayer(GetPlayerWithId(playerId));
        simIniTime = 0;
        ownPlayer = null;
        isConnected = false;
    }

	Message ReadServerMessage(BitBuffer bitBuffer) {
		MessageType messageType = bitBuffer.GetEnum<MessageType>((int) MessageType.TOTAL);
		Message serverMessage = null;

		switch (messageType) {
		    case MessageType.PLAYER_CONNECTED:
			    serverMessage = PlayerConnectedMessage.CreatePCMToReceive(); break;
		    case MessageType.PLAYER_DISCONNECTED:
			    serverMessage = PlayerDisconnectedMessage.CreatePDMToReceive(); break;
		    case MessageType.SNAPSHOT:
			    serverMessage = new SnapshotMessage(); break;
		    case MessageType.ACK_RELIABLE_MAX_WAIT_TIME:
			    serverMessage = AckReliableMessage.CreateAckReliableMessageToReceive(); break;
		    case MessageType.ACK_RELIABLE_SEND_EVERY_PACKET:
			    serverMessage = AckReliableSendEveryFrameMessage.CreateAckReliableSEFMToReceive(); break;
		    default:
			    Debug.LogError("Unknown server message received.");
			    return null;
		}

		serverMessage.Load(bitBuffer);
		return serverMessage;
	}

	void ProcessServerMessage(Message serverMessage) {
		switch (serverMessage.Type) {
		    case MessageType.PLAYER_CONNECTED:
			    ProcessPlayerConnected(serverMessage as PlayerConnectedMessage); break;
		    case MessageType.PLAYER_DISCONNECTED:
			    ProcessPlayerDisconnected(serverMessage as PlayerDisconnectedMessage); break;
		    case MessageType.SNAPSHOT:
			    ProcessSnapshot(serverMessage as SnapshotMessage); break;
		}
	}

	public void ProcessPlayerConnected(PlayerConnectedMessage message) {
		PlayerNetworkView player = GetPlayerWithId(message.PlayerId);
		if (player != null) {
			DisconnectPlayer(player);
		}
		ConnectPlayer(message.PlayerId);
	}

	public void ProcessPlayerDisconnected(PlayerDisconnectedMessage message) {
		PlayerNetworkView player = GetPlayerWithId(message.PlayerId);
		if (player != null) {
			DisconnectPlayer(player);
		}
	}	

	public void ProcessSnapshot(SnapshotMessage snapshot) {
		GameData gameData = snapshot.GameSnapshot;
        simIniTime = simIniTime == 0 ? gameData.Time : simIniTime;
        snapshots.Add(gameData);
		snapshots.OrderBy(snap => snap.Time).ToList();
	}

	public void Interpolate() {

		UpdateSpeed(); 
		simTime += Time.deltaTime * simSpeed;
		RemoveOldSnapshots();
		UpdateSpeed();

		if (snapshots.Count > 1) {
			GameData start;
			GameData end;
			if (maxDiffTime < snapshots[snapshots.Count - 1].Time - simIniTime - simTime) {
				start = snapshots [snapshots.Count - 1];
                end = snapshots[snapshots.Count - 1];
                simTime = end.Time - simIniTime;
			} else {
				start = snapshots[0];
				end = snapshots[1];
			}

			GameData interpolated = InterpolateData(start, end);
			List<PlayerData> playersData = interpolated.Players;
			foreach (PlayerData data in playersData) {
				int playerId = data.PlayerId;
				PlayerNetworkView player = GetPlayerWithId(playerId);
				if (player == null) {
					ConnectPlayer(playerId);
				}
				player.UpdatePosition(data.Position);
			}
		}
	}

	private GameData InterpolateData(GameData start, GameData end) {
		if (end.Time == start.Time) {
			return end;
		}
		float t = (float)(simTime -(start.Time - simIniTime)) /(end.Time - start.Time);
		List<PlayerData> interpolatedPlayersData = new List<PlayerData>();
		// TODO : Player is gone from one snap to another
		for (int i = 0; i < end.Players.Count; i++) {
			PlayerData endPlayer = end.Players[i];
			Vector2 interPosition = Vector2.zero;
			foreach (PlayerData playerData in start.Players) {
				if (playerData.PlayerId.Equals(endPlayer.PlayerId)) {
					interPosition = Vector2.Lerp(playerData.Position, endPlayer.Position, t);
					break;
				}
			}
            interpolatedPlayersData.Add(new PlayerData {
                PlayerId = endPlayer.PlayerId,
                Position = interPosition
            });
		}
        return new GameData {
            PlayersData = interpolatedPlayersData,
            Time = (float) simTime
        };
	}

	private void RemoveOldSnapshots() {
		if (snapshots.Count == 0) {
			return;
		}
		GameData prev = null;
		GameData interpolated = snapshots[0];

		while (snapshots.Count > 0 && simTime > interpolated.Time - simIniTime) {
			prev = interpolated;
			interpolated = snapshots[0];
            if (simTime > interpolated.Time - simIniTime) {
                snapshots.RemoveAt(0);
            }
        }

		if (prev != null) {
			snapshots.Insert(0, prev);
		}
	}

	private void UpdateSpeed() {
		double normal = 1 - 1.0 /(Mathf.Abs(buffDesiredLength - snapshots.Count) + 1);
		double max = 1.1;
		double factor = (max - 1) * normal + 1;
		if (snapshots.Count < buffDesiredLength) {
            simSpeed = simSpeed > 1 ? 1 / factor : simSpeed / factor;
		} else if (snapshots.Count > buffDesiredLength) {
            simSpeed = simSpeed < 1 ? factor : simSpeed * factor;
        } else {
			simSpeed = 1;
		}
	}

	private void ConnectPlayer(int _playerId) {
		PlayerNetworkView player = GetPlayerWithId(_playerId);
		if (player != null) {
			DisconnectPlayer(player);
		}

		GameObject playerGO = Instantiate(playerPrefab) as GameObject;
		playerGO.name = PLAYER + " " + _playerId; 
		player = playerGO.GetComponent<PlayerNetworkView>();
		player.id = _playerId;

		if (_playerId.Equals(playerId)) {
			isConnected = true;
			ownPlayer = playerGO.AddComponent<ClientPlayerController>();
			ownPlayer.playerInput = new PlayerInput();
		}
		players.Add(player);
	}

	private void DisconnectPlayer(PlayerNetworkView player) {
		Destroy(player.gameObject);
		players.Remove(player);
	}

	private PlayerNetworkView GetPlayerWithId(int playerId) {
		for (int i = 0; i < players.Count; i++) {
			if (playerId == players[i].id) {
				return players[i];
			}
		}
		return null;
	}

	private void ReadMessages() {
        ParsePackets();
        ProcessMessage();
	}

    private void ParsePackets() {
        Packet inPacket;
        while ((inPacket = channel.GetPacket()) != null) {
            BitBuffer bitBuffer = inPacket.buffer;
            int messageCount = bitBuffer.GetInt();
            for (int i = 0; i < messageCount; i++) {
                Message serverMessage = ReadServerMessage(bitBuffer);
                if (serverMessage != null) {
                    communicationManager.ReceiveMessage(serverMessage);
                }
            }
        }
    }

    private void ProcessMessage() {
        Message inMessage;
        while ((inMessage = communicationManager.GetMessage()) != null) {
            ProcessServerMessage(inMessage);
        }
    }
}
