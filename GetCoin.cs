using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetCoin : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinText;

    private void FixedUpdate()
    {
        int getCoins = GameManager.Instance.coins;
        coinText.text = string.Format("{0}", getCoins);
    }
}
