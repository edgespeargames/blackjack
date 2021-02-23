using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Action action = Action.Hit;
    public List<GameObject> hand = new List<GameObject>();
    private int handTotal = 0;
    public Transform[] spawnPoints;
    private int nextspawn = 0;

    public void Begin()
    {
        hand.Add(GameLogic.Instance.DrawCard(spawnPoints[0].transform.position));
        hand.Add(GameLogic.Instance.DrawCard(spawnPoints[1].transform.position));
        nextspawn = 2;
    }

    public void PlayerDrawCard()
    {
        hand.Add(GameLogic.Instance.DrawCard(spawnPoints[nextspawn].transform.position));
        nextspawn++;
    }

    public int GetNextSpawn()
    {
        return nextspawn;
    }

    public int HandValue()
    {
        handTotal = 0;

        for (int i = 0; i < hand.Count; i++)
        {
            int value = hand[i].gameObject.GetComponent<Card>().value;

            switch (value) //If it's Jack Queen or King
            {
                case 11:
                    value = 10;
                    break;
                case 12:
                    value = 10;
                    break;
                case 13:
                    value = 10;
                    break;
            }

            if (value == 1) // If it's an Ace
            {
                if ((handTotal + 11) > 21) //If we'll go bust
                {
                    handTotal += value;
                }
                else
                {
                    handTotal += 11;
                }
            }
            else
            {
                handTotal += value;
            }
        }
        return handTotal;
    }

    public void ResetHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            Destroy(hand[i]);
        }
        hand.Clear();
    }
}
