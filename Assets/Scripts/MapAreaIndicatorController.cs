using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAreaIndicatorController : MonoBehaviour {
    [SerializeField] private float offset_x = 0.0f;
    [SerializeField] private float offset_y = 0.0f;
    private const int MAPSIZE_MAX = 10;

    private GameObject origin;

    // Build MapArea Indicators
    void Start() {

        var map = GameObject.Find("Map_");
        if (map == null) return;
        gameObject.transform.parent = map.transform;
        gameObject.transform.localPosition = Vector3.zero;
    }
}
