using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    public static event Action joinButtonClicked;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => joinButtonClicked?.Invoke());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
