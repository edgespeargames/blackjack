using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public enum Action { Stand, Hit, Surrender, DoubleDown, Split, Win, Lose, Draw }

public class GameLogic : MonoBehaviour
{
    private Deck gameDeck;
    [SerializeField] private GameObject cardObj;

    [SerializeField]private Dealer dealer;
    [SerializeField]private Player player;

    [SerializeField] private Text dealerScoreText;
    [SerializeField] private Text playerScoreText;

    [SerializeField] private Text infoText;

    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button doubleButton;
    [SerializeField] private Button surrenderButton;

    [SerializeField] private GameObject bettingSystemObj;
    private BettingSystem bettingSystem;

    #region Singleton & Game Deck
    private static GameLogic _instance;

    public static GameLogic Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        bettingSystem = bettingSystemObj.GetComponent<BettingSystem>();

        gameDeck = new Deck();
        gameDeck.LoadDeck();
    }
    #endregion

    private void Start()
    {
        StartBetting();
    }

    void StartBetting()
    {
        InfoText("Place your bets");
        ResetCardTable();
        ToggleAllButtons(false);
        bettingSystem.BeginBet();
    }
    
    public void StartPlaying()
    {
        ToggleAllButtons(true);
        dealer.Begin();
        player.Begin();
        UpdateScoreText();

        if (bettingSystem.CanDouble())
            doubleButton.interactable = true;
        else
            doubleButton.interactable = false;
    }

    void ResetCardTable()
    {
        player.ResetHand();
        dealer.ResetHand();
        gameDeck.LoadDeck();
    }

    void ToggleAllButtons(bool active)
    {
        Interactable(active, hitButton, standButton, doubleButton, surrenderButton);
    }

    public GameObject DrawCard(Vector2 loc, bool hide = false)
    {
        int index = Random.Range(1, gameDeck.deck.Count);
        GameObject cardobj = gameDeck.DrawCard(index, cardObj, loc, hide);
        
        return cardobj;
    }

    IEnumerator DrawFullHand()
    {
        InfoText("Player stands on " + player.HandValue());
        yield return new WaitForSeconds(1f);
        InfoText("Dealer draws");
        yield return new WaitForSeconds(1f);
        int location = 2;
        dealer.hand[1].gameObject.GetComponent<Card>().SetSprite();
        UpdateScoreText();
        yield return new WaitForSeconds(1f);


        if (dealer.HandValue() < 17)
        {
            dealer.action = Action.Hit;
            InfoText("Dealer total: " + dealer.HandValue());
        }
        else
        {
            dealer.action = Action.Stand;
            InfoText("Dealer total: " + dealer.HandValue());
            if (dealer.HandValue() > player.HandValue())
            {
                InfoText("Player Loses with " + player.HandValue());
                bettingSystem.LoseBet();

            }
            else if (dealer.HandValue() == player.HandValue())
            {
                InfoText("Draw, nobody wins.");
                bettingSystem.DrawBet();
            }
            else
            {
                InfoText("Player Wins with " + player.HandValue());
                bettingSystem.WinBet();
            }
            StartCoroutine(PauseAndReset());
        }

        

        while (dealer.action == Action.Hit)
        {
            dealer.hand.Add(DrawCard(dealer.spawnPoints[location].transform.position));
            UpdateScoreText();
            yield return new WaitForSeconds(1f);

            if (dealer.HandValue() == 21 && dealer.HandValue() != player.HandValue())
            {
                dealer.action = Action.Win;
                InfoText("Dealer total: " + dealer.HandValue());
                yield return new WaitForSeconds(1f);
                InfoText(infoText.text + "\nDealer 21");
                yield return new WaitForSeconds(1f);
                InfoText(infoText.text + "\nPlayer Loses");
                bettingSystem.LoseBet();
                StartCoroutine(PauseAndReset());

                yield break;
            }

            if (dealer.HandValue() > 21)
            {
                dealer.action = Action.Lose;
                InfoText("Dealer total: " + dealer.HandValue());
                yield return new WaitForSeconds(1f);
                InfoText(infoText.text + "\nDealer Bust");
                yield return new WaitForSeconds(1f);
                InfoText(infoText.text + "\nPlayer wins with " + player.HandValue());
                bettingSystem.WinBet();
                StartCoroutine(PauseAndReset());
                
                yield break;
            }

            if (dealer.HandValue() >= 17)
            {
                dealer.action = Action.Stand;
                InfoText("Dealer total: " + dealer.HandValue());
                yield return new WaitForSeconds(1f);
                InfoText(infoText.text + "\nDealer Stands");
                yield return new WaitForSeconds(1f);

                if (dealer.HandValue() > player.HandValue())
                {
                    InfoText(infoText.text + "\nPlayer Loses with " + player.HandValue());
                    dealer.action = Action.Win;
                    player.action = Action.Lose;
                    bettingSystem.LoseBet();
                }

                else if (dealer.HandValue() < player.HandValue())
                {
                    InfoText(infoText.text + "\nPlayer Wins with " + player.HandValue());
                    dealer.action = Action.Lose;
                    player.action = Action.Win;
                    bettingSystem.WinBet();
                }

                else if (dealer.HandValue() == player.HandValue())
                {
                    InfoText(infoText.text + "\nDraw, nobody wins.");
                    dealer.action = Action.Draw;
                    player.action = Action.Draw;
                    bettingSystem.DrawBet();
                }

                ToggleAllButtons(false);

                StartCoroutine(PauseAndReset());

                yield break;
            }

            location++;
        }
    }

    void UpdateScoreText()
    {
        dealerScoreText.color = Color.black;
        playerScoreText.color = Color.black;

        if (dealerScoreText == null)
            dealerScoreText.text = "0";
        else
            dealerScoreText.text = dealer.HandValue().ToString();

        if (playerScoreText == null)
            playerScoreText.text = "0";
        else
            playerScoreText.text = player.HandValue().ToString();

        if (dealer.HandValue() > 21)
            dealerScoreText.color = Color.red;
        if (player.HandValue() > 21)
            playerScoreText.color = Color.red;
    }

    void Interactable(bool active, Button a, Button b = null, Button c = null, Button d = null)
    {
        a.interactable = active;
        if(b != null)
            b.interactable = active;
        if(c != null)
            c.interactable = active;
        if(d != null)
            d.interactable = active;
    }

    public void InfoText(string text)
    {
        infoText.text = text;
    }

#if UNITY_EDITOR
    void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
#endif

    public IEnumerator PauseAndReset()
    {
        yield return new WaitForSeconds(2f);
        OnResetPress();
    }

#region Button Presses
    public void OnResetPress()
    {
        StartBetting();
        UpdateScoreText();
#if UNITY_EDITOR
        ClearLog();
#endif
    }

    public void OnHitPress()
    {
        if (player.GetNextSpawn() > 5)
        {
            player.action = Action.Stand;
            StartCoroutine(DrawFullHand());
            ToggleAllButtons(false);
        }
        else
        {
            doubleButton.interactable = false;
            player.PlayerDrawCard();

            if (player.HandValue() == 21)
            {
                player.action = Action.Win;
                InfoText("Player Wins with " + player.HandValue());
                //StartCoroutine(DrawFullHand());
                bettingSystem.WinBet();
                StartCoroutine(PauseAndReset());
            }
            if (player.HandValue() > 21)
            {
                player.action = Action.Lose;
                InfoText("Player Loses with " + player.HandValue());
                bettingSystem.LoseBet();
                StartCoroutine(PauseAndReset());
            }
            if (player.action == Action.DoubleDown)
            {
                player.action = Action.Stand;
                StartCoroutine(DrawFullHand());
            }
        }
        UpdateScoreText();
    }

    public void OnStandPress()
    {
        //Disable buttons
        //and begin dealer draw
        player.action = Action.Stand;
        ToggleAllButtons(false);
        StartCoroutine(DrawFullHand());
        UpdateScoreText();
    }

    public void OnDoubleDownPress()
    {
        //Double bet
        //Draw one more card
        //Dealer draws
        if(player.hand.Count == 2 && bettingSystem.CanDouble())
        {
            bettingSystem.DoubleBet();
            ToggleAllButtons(false);
            player.action = Action.DoubleDown;
            OnHitPress();
        }
    }

    public void OnSurrenderPress()
    {
        ToggleAllButtons(false);
        InfoText("Player surrenders");
        bettingSystem.Surrender();
        StartCoroutine(PauseAndReset());
    }
#endregion
}
