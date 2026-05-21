using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;
using TMPro;

public class ItemManager : Singleton<ItemManager>
{
    public SOInt coins;
    public TextMeshProUGUI textMeshProUGUI;

    private string goldenCoin = "Gold Coin";
    private string silverCoin = "Silver Coin";
    //private string bronzeCoin = "Bronze Coin";

    void Start()
    {
        Reset();
    }

    private void Reset()
    {
        coins.value = 0;
        textMeshProUGUI.text = "0"+ coins.value.ToString();
    }

    public void AddCoins(GameObject gameObject, int amount = 1)
    {
        if (gameObject.CompareTag(goldenCoin))
        {
            coins.value += 10;
        }
        else if (gameObject.CompareTag(silverCoin))
        {
            coins.value += 5;
        }
        else
        {
            coins.value += amount;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        //if (coins.value < 10)
        //{
        //    textMeshProUGUI.text = "0" + coins.value.ToString();
        //    //UiInGameManager.Instance.UpdateTextCoins("0"+coins.ToString());
        //    //UiInGameManager.UpdateTextCoins("0"+coins.ToString());
        //}
        //else
        //{
        //    textMeshProUGUI.text = coins.value.ToString();
        //    //UiInGameManager.Instance.UpdateTextCoins(coins.ToString());
        //    //UiInGameManager.UpdateTextCoins(coins.ToString());
        //}
    }
}
