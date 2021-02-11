using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoundingBoxController : MonoBehaviour {
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject mapAreaIndicator;

    
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        var position = gameObject.transform.position;
        if (map != null) {
            map.transform.position = position;
            map.transform.rotation = gameObject.transform.rotation;
            if (mapAreaIndicator != null) {
                mapAreaIndicator.transform.position = position;
                mapAreaIndicator.transform.rotation = gameObject.transform.rotation;
            }
            map.transform.localScale = gameObject.transform.localScale;
        }
    }
}
