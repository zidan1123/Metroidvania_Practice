using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogPanelView : MonoBehaviour
{
    private Transform m_Transform;
    private Transform parent_Transform;
    private Transform dialogBox_Transform;
    private Transform hint_Transform;
    
    private TextMeshProUGUI dialog_Text;

    public Transform M_Transform { get { return m_Transform; } }
    public Transform Parent_Transform { get { return parent_Transform; } }
    public Transform DialogBox_Transform { get { return dialogBox_Transform; } }
    public Transform Hint_Transform { get { return hint_Transform; } }

    public TextMeshProUGUI Dialog_Text { get { return dialog_Text; } }

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        dialogBox_Transform = m_Transform.Find("DialogBackground");
        parent_Transform = m_Transform.parent;
        //Debug.Log(parent_Transform.name);
        dialog_Text = m_Transform.Find("DialogBackground/Dialog_Text").GetComponent<TextMeshProUGUI>();
        hint_Transform = m_Transform.Find("HintText").GetComponent<Transform>();
    }
}
