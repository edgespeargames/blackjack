using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suit { Spades, Hearts, Clubs, Diamonds }

public class Card : MonoBehaviour
{
    public int value { get; set; }

    public Suit suit { get; set; }

    [SerializeField] private Sprite[] spadeArray;
    [SerializeField] private Sprite[] heartArray;
    [SerializeField] private Sprite[] clubArray;
    [SerializeField] private Sprite[] diamondArray;

    [SerializeField] private Sprite cardRear;

    public void SetSprite()
    {
        switch (suit)
        {
            case Suit.Spades:
                GetComponent<SpriteRenderer>().sprite = spadeArray[value-1];
                break;
            case Suit.Hearts:
                GetComponent<SpriteRenderer>().sprite = heartArray[value-1];
                break;
            case Suit.Clubs:
                GetComponent<SpriteRenderer>().sprite = clubArray[value-1];
                break;
            case Suit.Diamonds:
                GetComponent<SpriteRenderer>().sprite = diamondArray[value-1];
                break;
            default:
                GetComponent<SpriteRenderer>().sprite = cardRear;
                break;
        }
    }

    public void HideCard()
    {
        GetComponent<SpriteRenderer>().sprite = cardRear;
    }

    public bool IsHidden()
    {
        if (GetComponent<SpriteRenderer>().sprite == cardRear)
            return true;
        else
            return false;
    }

    public string CardValue()
    {
        string stringValue = value.ToString();
        switch (value)
        {
            case 1:
                stringValue = "Ace";
                break;
            case 11:
                stringValue = "Jack";
                break;
            case 12:
                stringValue = "Queen";
                break;
            case 13:
                stringValue = "King";
                break;
        }

        return stringValue + " of " + suit.ToString();
    }

}
