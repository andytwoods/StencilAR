using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicModeToggle : MonoBehaviour
{
    public GameObject[] minimal_elements_to_hide;
    public Toggle toggle;

    private readonly String BASIC_MODE = "basic_mode";


    void Start()
    {
        toggle.onValueChanged.AddListener((value) => SetBasicMode(toggle, value));
        bool is_basic_mode = PlayerPrefs.GetInt(BASIC_MODE, 0) == 1; // defaults to 0
        toggle.isOn = is_basic_mode;
        ToggleBasicMode(is_basic_mode);
    }

    private void SetBasicMode(Toggle toggle, bool state)
    {
        PlayerPrefs.SetInt(BASIC_MODE, state ? 1: 0);
        ToggleBasicMode(state);
    }

    private void ToggleBasicMode(bool is_basic_mode)
    {
        foreach(GameObject g in minimal_elements_to_hide) {
            g.SetActive(is_basic_mode);
        }
    }


}
