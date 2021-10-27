using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    public GameManager gameManager;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gameManager.currencyManager.IncrementPixelPoints();
            gameManager.uiManager.UpdateWorkerUpgradeButtons();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gameManager.currencyManager.IncrementPrestigePoints();
            gameManager.uiManager.UpdateWorkerUpgradeButtons();
        }
    }
}
