using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput {

	public bool up;
	public bool down;
	public bool left;
	public bool right;
	public bool shoot;

	public PlayerInput() {
	}

	public PlayerInput(bool _up, bool _down, bool _left, bool _right, bool _shoot) {
		up = _up;
		down = _down;
		left = _left;
		right = _right;
		shoot = _shoot;
	}

	public void Load(BitBuffer buffer) {
		up = buffer.GetBit();
		down = buffer.GetBit();
		left = buffer.GetBit();
		right = buffer.GetBit();
		shoot = buffer.GetBit();
	}

	public void Save(BitBuffer buffer) {
		buffer.PutBit(up);
		buffer.PutBit(down);
		buffer.PutBit(left);
		buffer.PutBit(right);
		buffer.PutBit(shoot);
	}
}
