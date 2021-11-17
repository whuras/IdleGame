using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    public GameManager gameManager;
    public int givePixelPoints = 10;
    public int givePrestigePoints = 10;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gameManager.currencyManager.IncrementPixelPoints(givePixelPoints);
            gameManager.uiManager.UpdateWorkerUpgradeButtons();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gameManager.currencyManager.IncrementPrestigePoints(givePrestigePoints);
            gameManager.uiManager.UpdateWorkerUpgradeButtons();
        }
    }
}
