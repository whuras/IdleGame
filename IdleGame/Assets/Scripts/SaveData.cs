using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int currentLevel;
    public bool automationEnabled;
    public bool recycleEnabled;
    public bool customColorEnabled;
    public int customLockedCounter;
    public int prestigePoints;
    public int startingPixelPoints;
    public List<Vector3> goals; // total 256^3 colors
}
