using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPanelController : MonoBehaviour
{
    public static DialogPanelController Instance;

    private DialogPanelModel m_DialogPanelModel;
    private DialogPanelView m_DialogPanelView;

    [SerializeField] private int currentDialogID, currentLine;
    [SerializeField] List<string> currentDialogStringList;  //��ǰ�Ի�ϵͳ�ľ���
    [SerializeField] bool currentDialogRangeIsEntered;  //��ǰ�����Ƿ��ڵ�ǰ�Ի����ӿ��Դ����ķ�Χ

    void Start()
    {
        Init();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(m_DialogPanelView.Parent_Transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W) && currentDialogRangeIsEntered && !m_DialogPanelView.DialogBox_Transform.gameObject.activeInHierarchy)
        {
            currentLine = 0;
            DialogPanelShow();
            ShowDialog();
            FreezePlayer();
        }

        if (Input.GetMouseButtonUp(0) && m_DialogPanelView.DialogBox_Transform.gameObject.activeInHierarchy)
        {
            currentLine++;
            if (currentLine < currentDialogStringList.Count)
            {
                m_DialogPanelView.Dialog_Text.text = currentDialogStringList[currentLine];
            }
            else
            {
                DialogPanelHide();
                FreePlayer();
            }
        }
    }

    private void Init()
    {
        m_DialogPanelModel = gameObject.GetComponent<DialogPanelModel>();
        m_DialogPanelView = gameObject.GetComponent<DialogPanelView>();

        DialogPanelHide();
    }

    private void ShowDialog()
    {
        m_DialogPanelView.Dialog_Text.text = currentDialogStringList[currentLine];
    }

    public void SetCurrentDialog(int currentDialogID, bool isEntered)
    {
        this.currentDialogID = currentDialogID;
        this.currentDialogStringList = m_DialogPanelModel.GetStringListByDialogID(currentDialogID); 
        this.currentDialogRangeIsEntered = isEntered;
    }
    
    public void ResetCurrentDialog(bool isEntered)
    { 
        this.currentDialogRangeIsEntered = isEntered;
        DialogPanelHide();
    }

    public void DialogPanelShow()
    {
        m_DialogPanelView.DialogBox_Transform.gameObject.SetActive(true);
    }

    public void DialogPanelHide()
    {
        m_DialogPanelView.DialogBox_Transform.gameObject.SetActive(false);
    }

    private void FreezePlayer() //�����������о�����д��PlayerController��
    {
        FindObjectOfType<PlayerController>().CanMove = false;
        FindObjectOfType<PlayerController>().CanFlip = false;
        FindObjectOfType<PlayerController>().CanAttack = false;
    }
    
    private void FreePlayer() //
    {
        FindObjectOfType<PlayerController>().CanMove = true;
        FindObjectOfType<PlayerController>().CanFlip = true;
        FindObjectOfType<PlayerController>().CanAttack = true;
    }
}
