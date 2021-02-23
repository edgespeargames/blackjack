using UnityEngine;
using UnityEngine.UI;

public class BettingSystem : MonoBehaviour
{
    private int balance;
    public int maxBalance; //Maximum balance for the round
    public int bet = 0;
    [SerializeField] private Button[] buttons;

    [SerializeField] private Button resetButton;
    [SerializeField] private Button allInButton;
    [SerializeField] private Button playButton;

    [SerializeField] private Text balanceText;
    [SerializeField] private Text currentBetText;

    private string defaultBalance = "Balance: $";
    private string defaultBet = "Bet: $";

    void Start()
    {
        maxBalance = 1000;
        BeginBet();
    }

    public void BeginBet()
    {
        ResetBalanceAndBet();
        SetPlayButton(false);
        SetAllInButton(true);
        SetResetButton(false);
        SetButtons(true);
        SetChipInteractable();
    }

    private void SetResetButton(bool active)
    {
        resetButton.interactable = active;
    }

    private void SetAllInButton(bool active)
    {
        allInButton.interactable = active;
    }

    private void SetPlayButton(bool active)
    {
        playButton.interactable = active;
    }


    public void OnPlayPress()
    {
        SetPlayButton(false);
        SetAllInButton(false);
        SetResetButton(false);
        SetButtons(false);
        GameLogic.Instance.StartPlaying();
    }

    public void ResetBalanceAndBet()
    {
        balance = maxBalance;
        bet = 0;
        UpdateBalance();
        UpdateBet();
        SetChipInteractable();
    }

    public void AllIn()
    {
        bet = balance + bet;
        balance = 0;
        UpdateBalance();
        UpdateBet();
        SetChipInteractable();
        SetPlayButton(true);
        SetResetButton(true);
    }

    public void AddToBet(Button button)
    {
        int amount = button.gameObject.GetComponent<Chip>().value;

        if (balance - amount >= 0) //If taking the amount away from the current balance will not result in a negative balance
        {
            bet += amount; // Add to the current bet
            balance -= amount; // Remove from the current balance
            UpdateBalance(); //Update the balance
            UpdateBet(); //Update the bet
        }

        SetChipInteractable();

        if (bet > 0)
        {
            SetPlayButton(true);
            SetResetButton(true);
        }
        else
        {
            SetPlayButton(false);
            SetResetButton(false);
        }

    }

    private void SetButtons(bool active)
    {
        foreach (Button button in buttons)
        {
            button.interactable = active;
        }
    }

    public void SetChipInteractable()
    {
        foreach (Button button in buttons)
        {
            if (balance - button.gameObject.GetComponent<Chip>().value < 0)
                button.interactable = false;
            else
                button.interactable = true;
        }
    }

    void UpdateBalance()
    {
        balanceText.text = defaultBalance + balance.ToString();
    }

    void UpdateBet()
    {
        currentBetText.text = defaultBet + bet.ToString();
    }

    public void WinBet()
    {
        print(maxBalance);
        print(bet); //Bet is zero for some reason
        maxBalance = maxBalance + bet;
    }

    public void LoseBet()
    {
        print(maxBalance);
        print(bet);
        maxBalance = maxBalance - bet;
        if(maxBalance < 1)
        {
            GameLogic.Instance.InfoText("Player is out of money. \n House feels sorry for player.");
        }
        maxBalance = 1000;
    }

    public void DrawBet()
    {
        maxBalance = balance + bet;
    }

    //Check if the player has enough money to double down
    public bool CanDouble()
    {
        return bet * 2 <= maxBalance;
    }

    public void DoubleBet()
    {
        if (CanDouble())
        {
            balance = balance - bet;
            bet = bet * 2;
            UpdateBalance();
            UpdateBet();
        }
    }

    public void Surrender()
    {
        bet = (int)Mathf.Ceil(bet / 2);
        maxBalance = maxBalance - bet;
        bet = 0;
    }
}
