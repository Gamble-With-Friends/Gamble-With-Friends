using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public CanvasGroup editorCanvasGroup;
    public CanvasGroup connectingCanvasGroup;

    private void Start()
    {
        if (!Application.isBatchMode)
        {
            if(Application.isEditor)
            {
                editorCanvasGroup.gameObject.SetActive(true);
            } else
            {
                connectingCanvasGroup.gameObject.SetActive(true);
            }
        }
    }
}