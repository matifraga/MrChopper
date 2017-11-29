using UnityEngine;
using System.Net;

public class ServerPlayerController : MonoBehaviour {

	private int id;
	PlayerInput input = new PlayerInput();
	public IPEndPoint endPoint;
	public float maxSpeed;
	public bool showVisualRepresentation;
	public GameObject visualRepresentationGO;

	private CommunicationManager communicationManager = new CommunicationManager();
	private Transform ownTransform;

	public int Id {
		get {
			return id;
		}
		set {
			id = value;
		}
	}

    public PlayerInput Input {
        set {
            input = value;
        }
    }

    void Awake() {
		ownTransform = transform;
	}
	
	void Update () {
		ProcessMessages ();
		visualRepresentationGO.SetActive(showVisualRepresentation);
		int vertical = 0;
		int horizontal = 0;

		if (input.up) {
			vertical += 1;
		}
		if (input.down) {
			vertical -= 1;
		}
		if (input.left) {
			horizontal -= 1;
		}
		if (input.right) {
			horizontal += 1;
		}

		if (vertical > 0) {
			if (horizontal > 0) {
				ownTransform.forward = Vector3.up + Vector3.right;
			} else if (horizontal < 0) {
				ownTransform.forward = Vector3.up + Vector3.left;
			} else {
				ownTransform.forward = Vector3.up;
			}
		} else if (vertical < 0) {
			if (horizontal > 0) {
				ownTransform.forward = Vector3.down + Vector3.right;
			} else if (horizontal < 0) {
				ownTransform.forward = Vector3.down + Vector3.left;
			} else {
				ownTransform.forward = Vector3.down;
			}
		} else {
			if (horizontal > 0) {
				ownTransform.forward = Vector3.right;
			} else if (horizontal < 0) {
				ownTransform.forward = Vector3.left;
			}
		}

        if (Mathf.Abs (vertical) + Mathf.Abs (horizontal) > 0) {
			ownTransform.position += (ownTransform.forward * maxSpeed * Time.deltaTime);
		}

	}

    void ProcessMessages() {
        while (communicationManager.HasMessage()) {
            Message message = communicationManager.GetMessage();
            if (message.Type == MessageType.PLAYER_INPUT) {
                ProcessPlayerInput(message as PlayerInputMessage);
            }
        }
    }

	public PlayerData BuildPlayerData() {
		PlayerData playerData = new PlayerData ();
		playerData.PlayerId = id;
		playerData.Position = new Vector2 (ownTransform.position.x, ownTransform.position.y);
		return playerData;
	}

	public CommunicationManager CommunicationManager {
		get {
			return communicationManager;
		}
	}
		
	void ProcessPlayerInput(PlayerInputMessage playerInputMessage) {		
		Input = playerInputMessage.Input;
	}
}
