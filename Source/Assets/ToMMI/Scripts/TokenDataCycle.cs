using UnityEngine;
using System.Collections;
using Touchables;
using Touchables.TokenEngine;
using Touchables.MultiTouchManager;

public class TokenDataCycle : MonoBehaviour {

	void Start () {
	
	}
	
	void Update ()
    {
        //Cycle through Token Data
        foreach (Token t in InputManager.Tokens)
        {
            //Perform operations with Token Data
            //int tokenId = t.Id;
            //Vector2 tokenPosition = t.Position;
            //float tokenAngle = t.Angle;

        }

        foreach(FingerTouch finger in InputManager.Touches)
        {
            //Perform operations with finger touch data
            //TouchState fingerState = finger.State;
            //Vector2 fingerPosition = finger.Position;
        }
	
	}
}
