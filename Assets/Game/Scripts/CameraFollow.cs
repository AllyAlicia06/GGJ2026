using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth = 0.3f;
    public float fixedZ = -10f;
    
    private Vector3 velocity;

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, fixedZ);
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smooth);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
