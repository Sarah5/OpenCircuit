using UnityEngine;
using System.Collections;

public class Grab : MonoBehaviour {

	public int attach_index = 0;

	public float testDamper;
	public float testSpring;
	public float testMaxForce;

	private Player myPlayer;

	private float strength = 15;
	private const float holdDistance = 2f;
	public Vector3 normalHoldPosition = new Vector3(0.0f, -0.5f, holdDistance);
	private Vector3 holdPos;
	private ConfigurableJoint holdJoint;
	private GameObject grabbed = null;
	private JointDrive driver;
	
	void Awake () {
		myPlayer = this.GetComponent<Player> ();
	}

//	public Attach curAttach() {
//		if (grabbed == null)
//			return null;
//		Attach[] test = grabbed.GetComponentsInChildren<Attach> ();
//		if (test.Length > 0)
//			return test [attach_index];
//		return null;
//	}

	public bool GrabObject(GameObject obj, Vector3 holdPoint) {
		if (grabbed != null) {
			return false;
		}
		if (obj != null) {
			return holdObject(obj, holdPoint);
		}
		else return false;
	}
	
//	public void pocketObject(GameObject obj) {
//		if (grabbed != null) {
//			if (myPlayer.inventory.isEquiped(grabbed) != null) {
//				GameObject temp = grabbed;
//				releaseObject();
//				GameObject.Destroy(temp);
//			} else if (myPlayer.inventory.canCarry(grabbed)) {
//				GameObject item = grabbed;
//				dropObject();
//				myPlayer.inventory.addItem(item);
//			}
//			return;
//		}
//		if (obj != null)
//			this.GetComponent<Inventory>().addItem(obj);
//	}

//	public void dropObject() {
//		if (grabbed == null)
//			return;
//
//		Inventory.ItemStack stack = myPlayer.inventory.isEquiped(grabbed);
//		if (stack != null)
//			GameObject.Destroy(myPlayer.inventory.dropItem(stack));
//
//		releaseObject();
//	}

	private void releaseObject() {
		if (grabbed == null)
			return;
		Destroy (holdJoint);
		Physics.IgnoreCollision(GetComponent<Collider>(), grabbed.GetComponent<Collider>(), false);
		grabbed = null;
	}

	//public void Update() {
	//	if (holdJoint == null) return;
	//	driver.positionDamper = testDamper;
	//	driver.positionSpring = testSpring;
	//	driver.maximumForce = testMaxForce;
	//	holdJoint.xDrive = holdJoint.yDrive = holdJoint.zDrive = holdJoint.slerpDrive = driver;
	//}

	public bool holdObject(GameObject obj, Vector3 holdPoint) {
		grabbed = obj;

		Physics.IgnoreCollision(GetComponent<Collider>(), grabbed.GetComponent<Collider>(), true);

		// create and configure the joint
		driver = new JointDrive();
		driver.mode = JointDriveMode.Position;
		driver.positionDamper = strength /5;
		driver.positionSpring = strength *3f;
		driver.maximumForce = strength *3;

		holdJoint = grabbed.AddComponent<ConfigurableJoint>();
		holdJoint.autoConfigureConnectedAnchor = false;
		holdJoint.rotationDriveMode = RotationDriveMode.Slerp;
		//holdJoint.breakForce = 15f;

		holdJoint.connectedBody = this.gameObject.transform.GetChild(0).GetComponent<Rigidbody>();
		holdJoint.connectedAnchor = normalHoldPosition;
		if ((grabbed.GetComponent<Rigidbody>().worldCenterOfMass -holdPoint).sqrMagnitude > holdDistance / 2)
			holdJoint.anchor = grabbed.transform.InverseTransformPoint(holdPoint);
		else
			holdJoint.anchor = grabbed.GetComponent<Rigidbody>().centerOfMass;
		holdJoint.projectionDistance = 0;
		holdJoint.projectionAngle = 0;
		holdJoint.xDrive = holdJoint.yDrive = holdJoint.zDrive = holdJoint.slerpDrive = driver;
		holdJoint.targetPosition = Vector3.zero;
		return true;
	}

	public bool hasObject() {
		return (grabbed != null);
	}

	public Vector3 holdPosition() {
		return myPlayer.cam.transform.TransformPoint(normalHoldPosition);
	}

	public GameObject getCarried() {
		return grabbed;
	}

//	public void increment() {
//		if (grabbed == null)
//			return;
//		if (attach_index < grabbed.GetComponentsInChildren<Attach>().Length -1)
//			attach_index++;
//		else
//			attach_index = 0;
//	}
	
//	public void decrement() {
//		if (grabbed == null)
//			return;
//		if (attach_index > 0)
//			attach_index--;
//		else
//			attach_index = grabbed.GetComponentsInChildren<Attach>().Length -1;
//	}

	public void holdAside(bool aside) {
		if (grabbed == null)
			return;
		if (aside)
			holdJoint.connectedAnchor = normalHoldPosition +new Vector3(0, -0.5f, -holdDistance *0.2f);
		else
			holdJoint.connectedAnchor = normalHoldPosition;
	}

	public void rotateHeld() {

	}
	
}
