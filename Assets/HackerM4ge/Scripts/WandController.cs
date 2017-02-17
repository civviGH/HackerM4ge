using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour {


	// buttons
	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
	private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
	private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
	
	// controller initalize
	private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index);}}
	private SteamVR_TrackedObject trackedObj;
  
  
	// Use this for initialization
	void Start () {
    trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {

	}
}
