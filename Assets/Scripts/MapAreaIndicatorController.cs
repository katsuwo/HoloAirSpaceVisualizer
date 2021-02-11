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
/*       
        origin = GameObject.Find("MapArea");
        if (origin == null) return;
        var parent = GameObject.Find("MapAreaIndicator").transform;
        gameObject.transform.localPosition = new Vector3(offset_x, 0, offset_y);

        for (var i = 0; i <= MAPSIZE_MAX; i++) {
            var count = 0;
            var grpObj = new GameObject();
            grpObj.name = $"MapAreaGroup{i}";
            grpObj.transform.localPosition = new Vector3(0,0,0);
            grpObj.transform.position = new Vector3(0,0,0);
            for (var x = -i; x <= i; x++) {
                for (var y = -i; y <= i; y++) {
                    if ((x != -i && x != i) && (y != -i && y != i )) break;
                    CreateElement(x, y, i, grpObj, count++);
                }
            }

            grpObj.transform.parent = parent;
        }
*/        
    }
/*
    private void CreateElement(int x, int y, int grpNum, GameObject grpObj, int count) {
        var objectName = $"MapArea_{grpNum}_{count}";
        var newObject = GameObject.Instantiate(origin);
        newObject.transform.parent = grpObj.transform;
        newObject.name = objectName;
        newObject.transform.localScale = new Vector3(1,1,1);
        newObject.transform.localPosition = new Vector3((float)x + offset_x, 0, (float)y+ offset_y);
    }
*/    
    
    // Update is called once per frame
    void Update() {
        
    }
}
