using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    public Text instructionText;
    public RawImage instructionBackground;

    private void OnEnable()
    {
        EventManager.OnInstructionChange += HandleInstructionChange;
    }

    private void OnDisable()
    {
        EventManager.OnInstructionChange -= HandleInstructionChange;
    }

    private void HandleInstructionChange(string instruction)
    {
        instructionText.text = instruction;
        instructionBackground.enabled = !string.IsNullOrEmpty(instruction);
    }
}
