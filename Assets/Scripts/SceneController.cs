using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Object = System.Object;
using Random = UnityEngine.Random;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardSpawnPoint;
    [SerializeField] private Sprite[] cardImages;
    private List<Card> cards; // List of cards in the game

    private Card card1;
    private Card card2;
    private int score = 0;
    
    [SerializeField] private UIManager uiManager;
    
    private bool animationInProgress = false;

    private void Awake()
    {
        Messenger<Card>.AddListener(GameEvent.CARD_CLICKED, this.OnCardClicked);
    }

    private void OnDestroy()
    {
        Messenger<Card>.RemoveListener(GameEvent.CARD_CLICKED, this.OnCardClicked);
    }

    public void OnResetButtonPressed()
    {
        Reset();
    }
    
    private void Reset()
    {
        score = 0;
        uiManager.UpdateScore(score);
        card1 = null;
        card2 = null;
        animationInProgress = false; // Reset this flag

        // Reset the cards
        foreach (Card card in cards)
        {
            card.ResetCard();
        }

        // Assign images to the cards in pairs
        AssignImagesToCards();
    }

    // Create and Return a single Card at a given Position
    Card CreateCard(Vector3 position)
    {
        GameObject cardObj = Instantiate(cardPrefab, position, cardPrefab.transform.rotation);
        Card card = cardObj.GetComponent<Card>();
        return card;
    }

    // public void CardClicked(Card card)
    // {
    //     Debug.Log("Card clicked: " + this + " " + card.GetSprite());
    //     score++;
    //     uiManager.UpdateScore(score);
    // }

    private void OnCardClicked(Card card)
    {
        // Ignore clicks if an animation is already in progress
        if (animationInProgress)
            return;

        // Can we Flip the card?
        if (card1 == null)
        {
            card1 = card;
            animationInProgress = true;
            card1.FlipCard(true);
            StartCoroutine(WaitForAnimation());
        }
        else if (card2 == null)
        {
            card2 = card;
            animationInProgress = true;
            card2.FlipCard(true);
            StartCoroutine(EvaluatePair());
        }
    }
    
    private IEnumerator WaitForAnimation()
    {
        // Wait until the flip animation is complete
        yield return new WaitForSeconds(0.5f);
        animationInProgress = false;
    }

    IEnumerator EvaluatePair()
    {
        // Wait until the flip animation is complete
        yield return new WaitForSeconds(0.5f);

        // Do the cards Match?
        if (card1.GetSprite() == card2.GetSprite())
        {
            score++;
            Debug.Log("Score: " + score);
            uiManager.UpdateScore(score);

            // Prevent further clicks on matched cards
            card1.isMatched = true;
            card2.isMatched = true;

            card1 = null;
            card2 = null;
            animationInProgress = false;
        }
        else
        {
            // Store original positions
            Vector3 pos1 = card1.transform.position;
            Vector3 pos2 = card2.transform.position;

            // Wait before hiding and swapping the cards
            yield return new WaitForSeconds(1f);

            // Animate flipping the cards back while also swapping positions
            card1.FlipCard(false, pos2);
            card2.FlipCard(false, pos1);

            // Wait until the flip back animation completes
            yield return new WaitForSeconds(0.5f);

            // Reset the cards after hiding them
            card1 = null;
            card2 = null;
            animationInProgress = false;
        }
    }

    private void Start()
    {
        // Create a list of cards in a grid layout
        cards = CreateCards();

        // Assign images to the cards in pairs
        AssignImagesToCards();

        // Set the callback for the CardClicked event
        foreach (Card card in cards)
        {
            card.SetFaceVisible(false);
        }
    }

    // Create (and return) a List of cards organized in a grid layout
    private List<Card> CreateCards()
    {
        List<Card> newCards = new List<Card>();
        int rows = 2; // # of rows
        int cols = 4; // # of columns
        float xOffset = 2f; // # of units between cards horizontally
        float yOffset = -2.5f; // # of units between cards vertically

        // Create cards and position on a grid
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 offset = new Vector3(x * xOffset, y * yOffset, 0); // calculate the offset                
                Card card = CreateCard(cardSpawnPoint.position + offset); // create the card          
                newCards.Add(card); // add the card to the list
            }
        }

        return newCards;
    }

    // Assign images to the cards in pairs
    private void AssignImagesToCards()
    {
        // create a list of paired image indices - the # of entries MUST match the # of cards.
        // eg: [0,0,1,1,2,2,3,3]
        List<int> imageIndices = new List<int>();
        for (int i = 0; i < cardImages.Length; i++)
        {
            imageIndices.Add(i); // one index for the first card in the pair
            imageIndices.Add(i); // one index for the second
        }

        // Using the Fisher-Yates shuffle algorithm
        for (int i = imageIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // Swap elements
            (imageIndices[i], imageIndices[randomIndex]) = (imageIndices[randomIndex], imageIndices[i]);
        }


        // Go through each card in the game and assign it an image based on the (shuffled) list of indices.
        for (int i = 0; i < cards.Count; i++)
        {
            int imageIndex = imageIndices[i]; // use the card # to index into the imageIndices array
            cards[i].SetSprite(cardImages[imageIndex]); // set the image on the card
        }
    }
}