using UnityEngine;

public class CanvasWiper : MonoBehaviour
{
    public RenderTexture screenCanvas;

    void Start()
    {
        
        RenderTexture.active = screenCanvas;
        GL.Clear(true, true, Color.black); 
        RenderTexture.active = null;
    }
}