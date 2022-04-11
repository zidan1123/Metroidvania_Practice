using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogPanelView : MonoBehaviour
{
    private Transform m_Transform;
    private Transform parent_Transform;
    private Transform dialogBox_Transform;
    
    private TextMeshProUGUI dialog_Text;

    public Transform M_Transform { get { return m_Transform; } }
    public Transform Parent_Transform { get { return parent_Transform; } }
    public Transform DialogBox_Transform { get { return dialogBox_Transform; } }

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
        Debug.Log(parent_Transform.name);
        dialog_Text = GameObject.Find("DialogCanvas/DialogPanel/DialogBackground/Dialog_Text").GetComponent<TextMeshProUGUI>();

    }
}
