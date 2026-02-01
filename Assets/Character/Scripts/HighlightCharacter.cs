
using UnityEngine;

public class HighlightCharacter : MonoBehaviour
{
    public void SetHighlight(bool highlight)
    {
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Debug.Log("HighlightCharacter: SetHighlight called with " + highlight + " on " + gameObject.name + " spriteRenderer: " + spriteRenderer);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = highlight ? Color.yellow : Color.white;
            return;
        }
        
        
    }
}
