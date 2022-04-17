using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public sealed class JsonTools : MonoBehaviour
{
    public static void WriteSingleGroupJson(string path, string json)
    {
        Debug.Log(json);
        File.WriteAllText(path, json);
    }
}
