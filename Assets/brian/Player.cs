using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {


	public Texture2D coldTex;
	public float coldStart;
	public float coldExtra;
	public Color coldColor;
	public float freezeTemp;
	public bool drawReticle;
	public Texture2D reticle;
	public float maxOxygen = 60;
	public float oxygenRecoveryRate = 2;
	public float oxygen = 60;
	public AudioClip heavyBreathingSound;
	public AudioClip teleportSound;
	public float whiteOutDuration;
	public float blackOutDuration;
	
	/*private Attacher myAttacher;
	private Attack myAttacker;
	private Equip myEquipper;
	private Focus myFocus;
	private Grab myGrabber;
	private Interact myInteractor;
	private Inventory myInventory;*/
	private Movement myMover;
	private Camera myCam;
	private MouseLook myLooker;
	/*private Heat myHeat;
	private Hunger myHunger;
	private MapViewer myMapViewer;*/
	private Controls myControls;

	private static Player instance;
	private AudioSource breathingSource;
	private float whiteOutTime;
	private float blackOutTime;
	private Texture2D whiteOutTexture;

/*	public Attacher attacher { get { return myAttacher; } set { myAttacher = value; } }
	public Attack attacker { get { return myAttacker; } set { myAttacker = value; } }
	public Equip equipper { get { return myEquipper; } set { myEquipper = value; } }
	public Focus focus { get { return myFocus; } set { myFocus = value; } }
	public Grab grabber { get { return myGrabber; } set { myGrabber = value; } }
	public Interact interactor { get { return myInteractor; } set { myInteractor = value; } }
	public Inventory inventory { get { return myInventory; } set { myInventory = value; } }*/
	public Movement mover { get { return myMover; } set { myMover = value; } }
	public Camera cam { get { return myCam; } set { myCam = value; } }
	public MouseLook looker { get { return myLooker; } set { myLooker = value; } }
	/*public Heat heat { get { return myHeat; } set { myHeat = value; } }
	public Hunger hunger { get { return myHunger; } set { myHunger = value; } }
	public MapViewer mapViewer { get { return myMapViewer; } set { myMapViewer = value; } }*/
	public Controls controls { get { return myControls; } set { myControls = value; } }

	public static Player getInstance() {
		return Player.instance;
	}

	void Awake() {
		Player.instance = this;
		/*attacher = GetComponent<Attacher>();
		attacker = GetComponent<Attack> ();
		equipper = GetComponent<Equip>();
		focus = GetComponent<Focus>();
		grabber = GetComponent<Grab>();
		interactor = GetComponent<Interact>();
		inventory = GetComponent<Inventory>();*/
		mover = GetComponent<Movement>();
		cam = GetComponentInChildren<Camera>();
		looker = GetComponentInChildren<MouseLook>();
		/*heat = GetComponent<Heat>();
		hunger = GetComponent<Hunger>();
		mapViewer = GetComponent<MapViewer>();*/
		controls = GetComponent<Controls>();
		
		coldStart = 80;
		coldExtra = 0.1f;
		coldColor = new Color(0.25f, 0.5f, 1);
		freezeTemp = -20;
		whiteOutTime = 0;
		breathingSource = gameObject.AddComponent<AudioSource>();
		breathingSource.clip = heavyBreathingSound;
		breathingSource.enabled = true;
		breathingSource.loop = true;
		whiteOutTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		whiteOutTexture.SetPixel (0, 0, Color.white);
	}

	void Update () {

		if (oxygen < maxOxygen - oxygenRecoveryRate *Time.deltaTime) {
			oxygen += oxygenRecoveryRate *Time.deltaTime;
			if (!breathingSource.isPlaying) {
				breathingSource.Play();
			}
			breathingSource.volume = Mathf.Max(0, 1 -oxygen /maxOxygen);
		} else {
			oxygen = maxOxygen;
			if (breathingSource.isPlaying) {
				breathingSource.Stop();
			}
		}
	}

	public void physicsPickup(GameObject item) {

	}

	public void lockMovement() {
		mover.lockMovement();
	}

	public void fadeIn() {
		blackOutTime = blackOutDuration;
	}

	public void teleport(Vector3 position) {
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		breathingSource.PlayOneShot(teleportSound);
		transform.position = position;
		whiteOutTime = whiteOutDuration;
	}

	void OnGUI () {
		// draw the reticle
		if (drawReticle) {
			Rect reticleShape = new Rect(Screen.width/2 -10, Screen.height/2 -8, 16f,16f);
			GUI.DrawTexture(reticleShape, reticle);
		}

		// draw the "coldness" screen shader

		if (whiteOutTime > 0) {
			GUI.color = new Color(1, 1, 1, whiteOutTime /whiteOutDuration *2);
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), whiteOutTexture);
			GUI.color = Color.white;
			whiteOutTime -= Time.deltaTime;
			if (whiteOutTime < 0)
				whiteOutTime = 0;
		}

		if (blackOutTime > 0) {
			GUI.color = new Color(0, 0, 0, blackOutTime / blackOutDuration *1.5f);
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), whiteOutTexture);
			GUI.color = Color.white;
			blackOutTime -= Time.deltaTime;
			if (blackOutTime < 0)
				blackOutTime = 0;
		}
		// draw the player temperature
		//GUI.Label (new Rect (10, 10, 100, 100), heat.ToString());

		//// draw the player's oxygen level
		//GUI.Label(new Rect(10, 30, 100, 20), oxygen.ToString());
	}
}
