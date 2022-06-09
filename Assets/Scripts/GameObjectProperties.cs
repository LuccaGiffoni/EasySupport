using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameObjectProperties : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var response = gameObject.GetComponent<TextMeshPro>();
        response.text = gameObject.name + ": " + gameObject.transform.localScale.ToString() + ", " + gameObject.transform.position.ToString();
    }
}
