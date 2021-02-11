using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handmenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EraseCube() {
        GameObject.Find("Cube").SetActive(false);
        Debug.Log("Erase");
    }
    public void ShowMessageA() {
        Debug.Log("A");
    }
}
