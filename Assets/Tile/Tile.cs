using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
	public interface Callback {
		void OnClick(int x, int y);
	}

	private int x;
	private int y;
	private Callback callback;

	public void Initialize(int x, int y, Callback callback) {
		this.x = x;
		this.y = y;
		this.callback = callback;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {
		this.callback.OnClick(this.x, this.y);
	}
}
