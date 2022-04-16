using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talkable : MonoBehaviour
{
    [SerializeField] private int dialogID;
    //[SerializeField] private bool isEntered;

    public float y_Offset;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //isEntered = true;
            DialogPanelController.Instance.SetCurrentDialog(dialogID);
            DialogPanelController.Instance.SetHintPositionAndOffset(gameObject.transform.position, y_Offset);
            Debug.Log(gameObject.transform.position + "-" + y_Offset);
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (FindObjectOfType<PlayerController>().IsGround)
            {
                //isEntered = true;
                DialogPanelController.Instance.SetIsEnteredToTrue();
                DialogPanelController.Instance.HintShow();
            }
            else
            {
                //isEntered = false;
                DialogPanelController.Instance.SetIsEnteredToFalse();
                DialogPanelController.Instance.HintHide();
            }

            DialogPanelController.Instance.SetHintPositionAndOffset(gameObject.transform.position, y_Offset);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //isEntered = false;
            DialogPanelController.Instance.SetIsEnteredToFalse();
            DialogPanelController.Instance.HintHide();
        }
    }
}
