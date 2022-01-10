using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates parsable object

[System.Serializable]
public class StateData
{
    public string Province_State = "";
    public string Country_Region = "";
    public long Confirmed = 0;
    public long Deaths = 0;
    public long Recovered = 0;

}