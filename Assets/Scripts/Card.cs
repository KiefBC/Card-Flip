using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private GameObject cardBack;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SceneController sceneController;

    // Flag to indicate if the card is matched and should ignore further clicks
    public bool isMatched = false;
    private bool isFaceUp = false;
    private bool isFlipping = false;

    // Reference to the card back SpriteRenderer
    [SerializeField] private SpriteRenderer cardBackRenderer; 

    // Layer names as constants
    private const string LAYER_FOREGROUND = "Foreground";
    private const string LAYER_CARD_FLIP = "Card Flip";

    public void SetFaceVisible(bool faceVisible)
    {
        // Back of the card is visible if faceVisible is false
        cardBack.SetActive(!faceVisible);
        isFaceUp = faceVisible;
    }

    private void OnMouseDown()
    {
        // Ignore clicks if the card is already matched, is currently shown, or is in the middle of flipping
        if (isMatched || isFaceUp || isFlipping)
        {
            return;
        }

        Messenger<Card>.Broadcast(GameEvent.CARD_CLICKED, this);
    }

    public void SetSprite(Sprite image)
    {
        spriteRenderer.sprite = image;
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }

    public void FlipCard(bool showFace, Vector3? targetPosition = null)
    {
        // Prevent multiple flip animations at the same time
        if (isFlipping) return;

        // Always flip if we're also moving
        if (targetPosition.HasValue || isFaceUp != showFace)
        {
            StartCoroutine(FlipCardRoutine(showFace, targetPosition));
        }
    }

    public void ResetCard()
    {
        // Stop any ongoing coroutines
        StopAllCoroutines();
        
        isMatched = false;
        isFaceUp = false;
        isFlipping = false;

        // Reset rotation and visibility
        transform.rotation = Quaternion.identity;
        SetFaceVisible(false);

        // Restore the original sorting layer
        SetSortingLayer(LAYER_FOREGROUND);
    }

    // Helper method to set the sorting layer for all renderers on this card
    private void SetSortingLayer(string layerName)
    {
        // Set layer for the card face (main sprite renderer)
        spriteRenderer.sortingLayerName = layerName;

        // Set layer for the card back if it has a renderer
        if (cardBackRenderer != null)
        {
            cardBackRenderer.sortingLayerName = layerName;
        }
        else
        {
            // If cardBackRenderer wasn't assigned in the Inspector, try to find it
            SpriteRenderer backRenderer = cardBack.GetComponent<SpriteRenderer>();
            if (backRenderer != null)
            {
                backRenderer.sortingLayerName = layerName;
            }
        }
    }

    private IEnumerator FlipCardRoutine(bool showFace, Vector3? targetPosition = null)
    {
        isFlipping = true;

        // Store original position if we're swapping
        Vector3 originalPosition = transform.position;
        bool isSwapping = targetPosition.HasValue;

        // Change the layer to Card Flip to ensure the card is rendered on top during the flip
        SetSortingLayer(LAYER_CARD_FLIP);

        // Duration for each half-flip
        float halfTime = 0.25f;

        // First phase: rotate to 90 degrees
        Quaternion startRotation = transform.rotation;
        Quaternion midRotation = Quaternion.Euler(0, 90, 0);

        float elapsedTime = 0;
        while (elapsedTime < halfTime)
        {
            transform.rotation = Quaternion.Slerp(startRotation, midRotation, elapsedTime / halfTime);

            // If we're swapping positions, also lerp the position
            if (isSwapping)
            {
                transform.position = Vector3.Lerp(originalPosition, targetPosition.Value, elapsedTime / (halfTime * 2));
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set card face at the middle point when card is edge-on and not visible
        SetFaceVisible(showFace);

        // Second phase: complete the flip to appropriate end rotation
        Quaternion endRotation = showFace ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);

        elapsedTime = 0;
        while (elapsedTime < halfTime)
        {
            transform.rotation = Quaternion.Slerp(midRotation, endRotation, elapsedTime / halfTime);

            // Continue position lerping during second half of animation
            if (isSwapping)
            {
                float totalProgress = 0.5f + (elapsedTime / (halfTime * 2));
                transform.position = Vector3.Lerp(originalPosition, targetPosition.Value, totalProgress);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we end at the exact rotation and position
        transform.rotation = endRotation;
        if (isSwapping)
        {
            transform.position = targetPosition.Value;
        }

        // Restore the original sorting layer
        SetSortingLayer(LAYER_FOREGROUND);

        isFlipping = false;
    }
}