using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player_GUI : MonoBehaviour
{
    TMP_Text batteryStatus;
    TMP_Text ExhaustedStatus;
    public bool IsKey = false;
    public int FusePosse = 0;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        batteryStatus = canvas.transform.Find("BatteryStatus").GetComponent<TMP_Text>();
        ExhaustedStatus = canvas.transform.Find("ExhaustedStatus").GetComponent<TMP_Text>();
    }
    public void UpdateBatteryValue(int value)
    {
        batteryStatus.text = "Battery: " + value + "%";
    }

    public void UpdateExhaustedStatus(int value)
    {
        ExhaustedStatus.text = "Exhausted: " + value + "%";
    }
}
