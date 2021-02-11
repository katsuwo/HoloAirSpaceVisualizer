using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{

    public static IEnumerator DelayMethod(int delayFrameCount, Action action) {
        for (var i = 0; i < delayFrameCount; i++) yield return null;
        action();
    }
}
