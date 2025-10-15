using System;
using TMPro;
using UnityEngine;

public class KeypadController: MonoBehaviour
{
    [SerializeField] private Unlockable unlockable;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private int maxLength = 4;

    private void Awake()
    {
        displayText.maxVisibleCharacters = maxLength;
        displayText.text = "";
    }

    public void ButtonPressed(string value)
    {
        if (value == "Enter")
        {
            unlockable.AttemptUnlock(displayText.text);
            return;
        }
        
        if (value == "Clear")
        {
            displayText.text = "";
            return;
        }
        
        displayText.text += value;
    }
}