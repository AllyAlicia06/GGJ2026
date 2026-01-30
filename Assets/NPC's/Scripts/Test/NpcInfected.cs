using System;
using UnityEngine;

public class NpcInfected : MonoBehaviour
{
    [Header("State")]
    public bool isInfected = true;
    
    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Color infectedColor = Color.red;
    public Color healthyColor = Color.green;

    private void Awake()
    {
        if(spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        ApplyVisual();
    }

    public void Infect()
    {
        isInfected = true;
        ApplyVisual();
    }

    public void Cure()
    {
        isInfected = false;
        ApplyVisual();
    }

    void ApplyVisual()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = isInfected ? infectedColor : healthyColor;
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
