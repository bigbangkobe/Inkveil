using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseChange : MonoBehaviour
{
    public Texture2D cursorTexture1;

    public Texture2D cursorTexture2;

    private CursorMode cursorMode = CursorMode.Auto;

    private Vector2 hotSpot = Vector2.zero;

    // Use this for initialization

    void Start()
    {

    }

    // Update is called once per frame

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {

            Cursor.SetCursor(cursorTexture2, hotSpot, cursorMode);

        }

        if (Input.GetMouseButtonUp(0))
        {

            Cursor.SetCursor(cursorTexture1, hotSpot, cursorMode);

        }
    }
}
