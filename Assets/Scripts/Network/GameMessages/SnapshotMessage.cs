public class SnapshotMessage : Message {
	private GameData gameData;

	public SnapshotMessage() : base(-1, MessageType.SNAPSHOT, ReliabilityType.UNRELIABLE) {
		gameData = new GameData ();
	}

	public SnapshotMessage(int id, GameData _gameData) : base(id, MessageType.SNAPSHOT, ReliabilityType.UNRELIABLE) {
		gameData = _gameData;
	}

	public override void Load (BitBuffer bitBuffer) {
		base.Load (bitBuffer);
		gameData.Load (bitBuffer);
	}

	public override void Save(BitBuffer bitBuffer) {
		base.Save(bitBuffer);
		gameData.Save (bitBuffer);
	}

	public GameData GameSnapshot {
		get {
			return gameData;
		}
	}
}