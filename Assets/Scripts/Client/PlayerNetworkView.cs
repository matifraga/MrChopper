using UnityEngine;

public class PlayerNetworkView : MonoBehaviour {

	public int id;

	public void UpdatePosition(Vector2 position) {
		transform.position = position;
	}
}
