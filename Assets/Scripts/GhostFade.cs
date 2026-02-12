using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GhostFade : MonoBehaviour {
    void Start() {
        
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.05f; 
        Destroy(gameObject, 0.8f); 
    }
}
