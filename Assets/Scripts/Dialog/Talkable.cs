using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talkable : MonoBehaviour
{
    [SerializeField] private int dialogID;
    [SerializeField] private bool isEntered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isEntered = true;
        DialogPanelController.Instance.SetCurrentDialog(dialogID, isEntered);
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        isEntered = false;
        DialogPanelController.Instance.ResetCurrentDialog(isEntered);
    }
}
