public class PlayerInputMessage : Message {
	private PlayerInput playerInput;

	public PlayerInputMessage() : base(-1, MessageType.PLAYER_INPUT, ReliabilityType.UNRELIABLE) {
		playerInput = new PlayerInput();
	}

	public PlayerInputMessage(int id, PlayerInput _playerInput) : base(id, MessageType.PLAYER_INPUT, ReliabilityType.UNRELIABLE) {
		playerInput = _playerInput;
	}

	public override void Load(BitBuffer bitBuffer) {
		base.Load(bitBuffer);
		playerInput.Load(bitBuffer);
	}

	public override void Save(BitBuffer bitBuffer) {
		base.Save(bitBuffer);
		playerInput.Save(bitBuffer);
	}

	public PlayerInput Input {
		get {
			return playerInput;
		}
	}
}
