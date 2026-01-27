using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public float movementSpeed = 5f;
    
    private Camera cam;
    private AudioListener listener;
    private CameraFollow camFollow;

    public override void OnNetworkSpawn()
    {
        cam = GetComponentInChildren<Camera>(true);
        listener = GetComponentInChildren<AudioListener>(true);
        camFollow = GetComponentInChildren<CameraFollow>(true);
        
        if (cam) cam.enabled = IsOwner;
        if(listener) listener.enabled = IsOwner;

        if (camFollow)
        {
            camFollow.enabled = IsOwner;
            if (IsOwner) camFollow.target = transform;
        }

        if (IsOwner)
        {
            GameObject mainObj = GameObject.Find("Main Camera");
            if (mainObj)
            {
                var mainCam = mainObj.GetComponent<Camera>();
                if(mainCam) mainCam.enabled = false;
                
                var mainListener = mainObj.GetComponent<AudioListener>();
                if(mainListener)  mainListener.enabled = false;   
            }
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        transform.position += new Vector3(x, y, 0) * movementSpeed * Time.deltaTime;
    }
}
