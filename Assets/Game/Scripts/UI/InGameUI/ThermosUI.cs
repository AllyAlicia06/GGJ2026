using System;
using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.UI;

public class ThermosUI : MonoBehaviour
{
    [SerializeField] private Image thermosImage;
    [SerializeField] private Sprite[] thermosSprites;
    
    private GameState gameState;

    private void OnPhaseChanged(int oldValue, int newValue) => Apply(newValue);
    
    private void Awake()
    {
        if(thermosImage == null)  thermosImage = GetComponent<Image>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = FindFirstObjectByType<GameState>();
        if (gameState == null) return;

        Apply(gameState.thermosPhase.Value);
        gameState.thermosPhase.OnValueChanged += OnPhaseChanged;
    }

    void OnDestroy()
    {
        if (gameState != null)
            gameState.thermosPhase.OnValueChanged -= OnPhaseChanged;
    }
    
    private void Apply(int phase)
    {
        if (thermosSprites == null || thermosSprites.Length == 0 || thermosImage == null) return;
        phase = Mathf.Clamp(phase, 0, thermosSprites.Length - 1);
        thermosImage.sprite = thermosSprites[phase];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
