using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DropShadow : MonoBehaviour
{
    [SerializeField] private Vector2 shadowOffset = new Vector2(0.1f, -0.1f);
    [SerializeField] private Material shadowMaterial;
    [SerializeField] private int orderOffset = -1;
    [SerializeField] private Color shadowColor = new Color(0, 0, 0, 0.5f);
    
    // Added field to keep shadow behind the card
    [SerializeField] private string shadowLayerName = "Background";

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer shadowSpriteRenderer;
    private GameObject shadowGameObject;
    private Card cardComponent;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cardComponent = GetComponent<Card>();
        CreateShadow();
    }

    private void CreateShadow()
    {
        // Create shadow gameobject as a child of this object
        shadowGameObject = new GameObject("Shadow");
        shadowGameObject.transform.parent = transform;
        shadowGameObject.transform.localPosition = (Vector3)shadowOffset;
        shadowGameObject.transform.localRotation = Quaternion.identity;

        // Create and setup shadow renderer
        shadowSpriteRenderer = shadowGameObject.AddComponent<SpriteRenderer>();
        shadowSpriteRenderer.sprite = spriteRenderer.sprite;

        // Use provided shadow material or create a default one
        if (shadowMaterial != null)
        {
            shadowSpriteRenderer.material = shadowMaterial;
        }
        else
        {
            shadowSpriteRenderer.color = shadowColor;
        }

        // Set sorting - always keep shadow in background layer
        shadowSpriteRenderer.sortingLayerName = shadowLayerName;
        shadowSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder + orderOffset;
    }

    void LateUpdate()
    {
        if (shadowSpriteRenderer != null && spriteRenderer != null)
        {
            // Update sprite if it changed
            if (shadowSpriteRenderer.sprite != spriteRenderer.sprite)
            {
                shadowSpriteRenderer.sprite = spriteRenderer.sprite;
            }

            // Always keep shadow in the background layer
            if (shadowSpriteRenderer.sortingOrder != spriteRenderer.sortingOrder + orderOffset)
            {
                shadowSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder + orderOffset;
            }
        }
    }

    // Add this method to enable/disable the shadow
    public void SetShadowActive(bool active)
    {
        if (shadowGameObject != null)
        {
            shadowGameObject.SetActive(active);
        }
    }
}