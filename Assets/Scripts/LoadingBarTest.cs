using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarTest : MonoBehaviour
{
    [SerializeField] GameObject progressBar;
    RectTransform rect;
    const float maxWidth = 20724.68f;

    [SerializeField] Slider slider;

    private void Start()
    {
        // slider.maxValue = 1.6f;
    }
    // Start is called before the first frame update
    void Update()
    {
        // slider.value += 0.005f;
    }
}
