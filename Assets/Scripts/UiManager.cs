using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] txt;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < txt.Length; i++)
        {
            txt[i].text = KeySetting.Keys[(KeyAction)i].ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < txt.Length; i++)
        {
            txt[i].text = KeySetting.Keys[(KeyAction)i].ToString();
        }
    }
}
