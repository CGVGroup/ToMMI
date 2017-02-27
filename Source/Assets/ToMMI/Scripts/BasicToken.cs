using UnityEngine;
using System.Collections;
using Touchables;
using System;
using Touchables.TokenEngine;

public class BasicToken : MonoBehaviour, ITokenEvents {


    //All token Events Are redirected Here

    public void OnTokenPlacedOnScreen(object sender, ApplicationTokenEventArgs e)
    {
        
    }

    public void OnTokenRemovedFromScreen(object sender, ApplicationTokenEventArgs e)
    {
        
    }

    public void OnTokenUpdated(object sender, ApplicationTokenEventArgs e)
    {
        
    }
}
