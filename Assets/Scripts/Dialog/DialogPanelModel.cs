using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using System.IO;

public class DialogPanelModel : MonoBehaviour
{
    private Dictionary<int, List<string>> dialogDic;

    void Awake()
    {
        dialogDic = GetDialogXlsx();
    }

    private Dictionary<int, List<string>> GetDialogXlsx()
    {
        string filePath = "Assets/Datas/DialogData.xlsx";

        FileInfo fileInfo = new FileInfo(filePath);

        ExcelPackage excelPackage = new ExcelPackage(fileInfo);
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

        Dictionary<int, List<string>> tempDic = new Dictionary<int, List<string>>();

        for (int i = 2; i < workSheet.Dimension.Rows + 1; i++)  //EPPlus索引从1开始
        {
            List<string> tempStringList = new List<string>();
            for (int j = 3; j < int.Parse(workSheet.Cells[i, 2].Value.ToString()) + 3; j++)  // j < 该行句子的数量 + 3
            {
                tempStringList.Add(workSheet.Cells[i, j].Value.ToString());
            }
            tempDic.Add(i - 1, tempStringList);
        }
        
        return tempDic;
    }

    public List<string> GetStringListByDialogID(int dialogID)
    {
        List<string> tempStringList;
        dialogDic.TryGetValue(dialogID, out tempStringList);
        return tempStringList;
    }

    /*
    private int maxSentenceCount = 4;

    private Dictionary<int, string[]> GetDialogCSVByName(string fileName)
    {
        Dictionary<int, string[]> tempDic = new Dictionary<int, string[]>();

        TextAsset textAssetData = Resources.Load<TextAsset>(fileName);
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);

        int columnCount = maxSentenceCount + 2;
        int tableSize = (data.Length / columnCount) - 1;  //这里data.Length一定不是rowCount的倍数

        for (int i = 0; i < tableSize; i++)
        {
            string[] tempStringArray = new string[] { data[columnCount * (i + 1)], data[columnCount * (i + 1) + 1], data[columnCount * (i + 1) + 2], data[columnCount * (i + 1) + 3] };
            tempDic.Add(i + 1, tempStringArray);
        }

        for (int i = 0; i < data.Length; i++)
        {
            Debug.Log(data[i]);
        }

        return tempDic;
    }

    public string[] GetStringArrayByDialogID(int dialogID)
    {
        string[] tempStringArray;
        dialogDic.TryGetValue(dialogID, out tempStringArray);
        return tempStringArray;
    }
    */
}
