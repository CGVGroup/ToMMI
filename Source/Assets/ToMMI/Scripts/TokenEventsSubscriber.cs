using UnityEngine;
using System.Collections;
using Touchables.TokenEngine;

public class TokenEventsSubscriber : MonoBehaviour {

    private TokenManager tm;
    

	void Start ()
    {

        tm = TokenManager.Instance;

        if (tm != null)
        {
            tm.TokenPlacedOnScreen += Tm_TokenPlacedOnScreen;
            tm.ScreenTokenUpdated += Tm_ScreenTokenUpdated;
            tm.TokenRemovedFromScreen += Tm_TokenRemovedFromScreen;
        }
	
	}

    // Update is called once per frame
    void Update () {
	
	}

    private void Tm_TokenRemovedFromScreen(object sender, ApplicationTokenEventArgs e)
    {
        Token token = e.Token;
    }

    private void Tm_ScreenTokenUpdated(object sender, ApplicationTokenEventArgs e)
    {
        Token token = e.Token;
    }

    private void Tm_TokenPlacedOnScreen(object sender, ApplicationTokenEventArgs e)
    {
        Token token = e.Token;
    }

}
