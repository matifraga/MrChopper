using System.Collections.Generic;

public class GameData {

	private float time;

	public GameData () {
		PlayersData = new List<PlayerData> ();
	}

	public void Save(BitBuffer bitBuffer) {
		bitBuffer.PutFloat(time);
		bitBuffer.PutInt(PlayersData.Count);
		for (int i = 0; i < PlayersData.Count; i++) {
			PlayersData [i].Save(bitBuffer); 
		}
	}

	public void Load(BitBuffer bitBuffer) {
		time = bitBuffer.GetFloat();
		int playerCount = bitBuffer.GetInt();
		for (int i = 0; i < playerCount; i++) {
			PlayerData playerData = new PlayerData();
			playerData.Load (bitBuffer);
			PlayersData.Add (playerData);
		}
	}

	public List<PlayerData> Players {
		get {
			return PlayersData;
		}
	}

	public float Time {
		get {
			return time;
		}
		set {
			time = value;
		}
	}
    public List<PlayerData> PlayersData { get; set; }

}
