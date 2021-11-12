using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager Instance { get => instance; }
    private static WorkerManager instance;

    [SerializeField] public Worker[] workers;

    private void Awake()
    {
        MaintainSingleInstance();
    }

    private void MaintainSingleInstance()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
}
