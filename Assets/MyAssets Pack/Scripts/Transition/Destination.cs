using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//>传送目的地 终点标签
public class Destination : MonoBehaviour
{
    public DestinationTag destinationTag;
    public enum DestinationTag
    {
        Home, GameStart, Boss, None1, None2, None3, None4, None5, None6
    }
}
