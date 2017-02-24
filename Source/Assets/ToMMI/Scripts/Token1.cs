using UnityEngine;
using System.Collections;
using Touchables;
using System;
using Touchables.TokenEngine;

public class Token1 : MonoBehaviour, ITokenEvents {

    public SpriteRenderer spriteRenderer;
    private Vector3 tokenPosition;


    public void OnTokenPlacedOnScreen(object sender, ApplicationTokenEventArgs e)
    {
        tokenPosition = new Vector3(e.Token.Position.x, e.Token.Position.y, Camera.main.nearClipPlane);
        transform.position = Camera.main.ScreenToWorldPoint(tokenPosition);
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, e.Token.Angle);
        spriteRenderer.enabled = true;
    }

    public void OnTokenRemovedFromScreen(object sender, ApplicationTokenEventArgs e)
    {
        spriteRenderer.enabled = false;
    }

    public void OnTokenUpdated(object sender, ApplicationTokenEventArgs e)
    {
        tokenPosition = new Vector3(e.Token.Position.x, e.Token.Position.y, Camera.main.nearClipPlane);
        transform.position = Camera.main.ScreenToWorldPoint(tokenPosition);
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, e.Token.Angle);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
