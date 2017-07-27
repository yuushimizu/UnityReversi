using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour {
	public bool isBlack {
		get {
			return GetComponent<Animator>().GetBool("isBlack");
		}
		private set {
			GetComponent<Animator>().SetBool("isBlack", value);
		}
	}

	// Use this for initialization
	void Start() {
	}
	
	// Update is called once per frame
	void Update() {
	}

	public void Put(bool isBlack) {
		this.isBlack = isBlack;
		GetComponent<Animator>().SetTrigger("put");
	}

	public void Reverse() {
		isBlack = !isBlack;
		GetComponent<Animator>().SetTrigger("reverse");
	}
}
