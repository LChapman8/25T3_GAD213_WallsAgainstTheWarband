using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture; // assign your texture here
    public Vector2 hotspot = Vector2.zero; // where the "click point" is
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
        }
        else
        {
            Debug.LogWarning("CursorManager: No cursor texture assigned!");
        }
    }
}
