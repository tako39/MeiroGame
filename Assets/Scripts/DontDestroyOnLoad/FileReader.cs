using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileReader : MonoBehaviour
{
    public static FileReader Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    public string FileRead(string path) //ファイルの読み込み
    {
        string readStream = "";

        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                readStream = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return readStream;
    }
}