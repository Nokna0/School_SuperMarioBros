using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImanager : MonoBehaviour
{
    public void OnRetryButton()
    {
        // ΩÃ±€≈Ê¿∏∑Œ GameManager ¡¢±Ÿ
        GameManager.Instance.ResetLevel(0f);
    }
}
