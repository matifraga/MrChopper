using utils;
using UnityEngine;

public class Player : MonoBehaviour {

    public float moveSpeed;

    void Start () {
        Debug.Log("Start player");
	}
	
	void Update () {
        if (Input.GetKey(KeyCode.A)) {
            transform.position += Time.deltaTime * moveSpeed * -transform.right;
        }

        if (Input.GetKey(KeyCode.D)) {
            transform.position += Time.deltaTime * moveSpeed * transform.right;
        }

        if (Input.GetKey(KeyCode.W)) {
            transform.position += Time.deltaTime * moveSpeed * transform.forward;
        }

        if (Input.GetKey(KeyCode.S)) {
            transform.position += Time.deltaTime * moveSpeed * -transform.forward;
        }
    }

    public void Save(BitBuffer bitBuffer) {

    }

    public void Read() {

    }
}
