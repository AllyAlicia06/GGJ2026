using UnityEngine;

public class NPC_Handler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private string _npcName = "NPC";
    public string NPCName { get { return _npcName; } set { _npcName = value; } }
    private bool _isInfected = false;
    public bool IsInfected { get { return _isInfected; } set { _isInfected = value; } }


    private float _healthyTemperature = 37.0f;
    public float HealthyTemperature { get { return _healthyTemperature; } set { _healthyTemperature = value; } }
    private float _infectedTemperature = 39.0f;
    public float InfectedTemperature { get { return _infectedTemperature; } set { _infectedTemperature = value; } }

    private void Awake()
    {
        _healthyTemperature= Random.Range(36.5f, 37.5f);
        _infectedTemperature= Random.Range(38.5f, 40.0f);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
