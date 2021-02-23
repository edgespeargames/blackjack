using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> deck = new List<Card>();

    public void LoadDeck()
    {
        for(int i = 0; i < 4; i++)
        {
            for (int j = 1; j < 14; j++)
            {
                Card card = new Card();
                card.value = j;
                card.suit = (Suit)i;
                deck.Add(card);
            }
        }
        
    }

    public GameObject DrawCard(int index, GameObject cardPrefab, Vector2 loc, bool hide)
    {
        Card card = new Card();
        card.value = deck[index].value;
        card.suit = deck[index].suit;

        GameObject cardObj = Instantiate(cardPrefab, loc, Quaternion.identity);
        cardObj.gameObject.GetComponent<Card>().value = card.value;
        cardObj.gameObject.GetComponent<Card>().suit = card.suit;

        if(hide)
            cardObj.gameObject.GetComponent<Card>().HideCard();
        else
            cardObj.gameObject.GetComponent<Card>().SetSprite();
        
        deck.RemoveAt(index);

        return cardObj;
    }

}
