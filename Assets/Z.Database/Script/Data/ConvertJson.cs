using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using Unity.VisualScripting.FullSerializer;
using ExcelDataReader;
//TODO : ���� ������ �߰��Ǹ� �ؾ��� �� 2
public enum EFolderName
{
    none,
    Item
}
public static class ConvertJson<T> where T : class , new()
{
    
    static string Mainpath = Path.Combine(Application.dataPath, "Resources", "Data","Json");
    static string ResourcesPath = Path.Combine("Data","Json");
    static List<string> PathFolderList = new List<string>{ "Item" };//TODO : ���� ������ �߰��Ǹ� �ؾ��� �� 3 �� 3��

    /// <summary>
    /// Json���� �����
    /// </summary>
    /// <param MonsterName="FolderName">������ �̸�</param>
    /// <param MonsterName="fileName">������ �̸�</param>
    /// <param MonsterName="obj">Ŭ����</param>
    public static void CreateJsonFile(EFolderName FolderName, string FileName, T obj)
    {
        if (!Directory.Exists(Mainpath)) Directory.CreateDirectory(Mainpath);
        if (!Directory.Exists(Path.Combine(Mainpath, PathFolderList[(int)FolderName - 1]))) 
            Directory.CreateDirectory(Path.Combine(Mainpath, PathFolderList[(int)FolderName - 1]));

            //����ó��1. obj�� null�� ���
            if (obj == null) { Debug.Log("ERROR : Ŭ������ null�Դϴ�"); return; }
        if (FolderName == EFolderName.none) {Debug.Log("���õ� foldername�� �����ϴ�"); return; }

        //���� ������ �������
        
        
        //path ����
        string curPath = Path.Combine(Mainpath, PathFolderList[(int)FolderName -1],FileName+".json");
        
        string jsonData = JsonConvert.SerializeObject(obj, Formatting.Indented);
        System.IO.File.WriteAllText(curPath.ToString(), jsonData);
    }

    /// <summary>
    /// Json���� �б�
    /// ������ �������� �ʴ´ٸ� Null�� ��ȯ
    /// </summary>
    /// <param MonsterName="FolderName">������ �̸�</param>
    /// <param MonsterName="FileName">������ �̸�</param>
    /// <returns></returns>
    public static T ReadJsonFile(EFolderName FolderName,string FileName) 
    {
        //����ó��1. FileName�� �����̾��� ���
        //if (FolderName == EFolderName.none) { Debug.Log("���õ� foldername�� �����ϴ�"); return null; }
        //TextAsset jsonTextAsset = Resources.Load<TextAsset>("data");
        //string curPath = Path.Combine(Mainpath, PathFolderList[(int)FolderName - 1], FileName + ".json");
        string resourcespath = Path.Combine(ResourcesPath, PathFolderList[(int)FolderName - 1], FileName);
        //if (!System.IO.File.Exists(curPath)) { Debug.Log($"ERROR : {FileName}�̶�� ������ �����ϴ�!"); return null; }



        //string ReadJsonstr = System.IO.File.ReadAllText(curPath);
        var g = Resources.Load<TextAsset>(resourcespath);
        var gg = JsonConvert.DeserializeObject<T>(g.ToString());
        
        return gg;
    }

    public static void NoClassCreateJsonFile(EFolderName FolderName, string FileName, List<Dictionary<string, string>> parsingDictionary)
    {
        if (!Directory.Exists(Mainpath)) Directory.CreateDirectory(Mainpath);
        if (!Directory.Exists(Path.Combine(Mainpath, PathFolderList[(int)FolderName - 1])))
            Directory.CreateDirectory(Path.Combine(Mainpath, PathFolderList[(int)FolderName - 1]));

        //����ó��1. obj�� null�� ���
        if (FolderName == EFolderName.none) { Debug.Log("���õ� foldername�� �����ϴ�"); return; }
        //path ����
        string curPath = Path.Combine(Mainpath, PathFolderList[(int)FolderName - 1], FileName + ".json");

        string jsonData = JsonConvert.SerializeObject(parsingDictionary, Formatting.Indented);
        System.IO.File.WriteAllText(curPath.ToString(), jsonData);
    }
        /// <summary>
        /// csvParsing
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
    //    public static List<Dictionary<string, string>> CsvParsing(string FileName)
    //{
    //    string Mainpath_ = Path.Combine(Application.dataPath, "30.DataBase/DataCsv");
    //    //List dictionary �ʱ�ȭ
    //    List<Dictionary<string, string>> parsingDictionary = new List<Dictionary<string, string>>(); //��ȯ�� �ڷᱸ��
    //    List<string> Key = new List<string>(); //�ݺ��ϸ� �۾��� key�� ����
    //    string curPath = Path.Combine(Mainpath_, FileName + ".csv");
    //    StreamReader stream = new StreamReader(curPath);

    //    //ù�� �б� : key�� ����
    //    string line = stream.ReadLine();
    //    string[] keys = line.Split(',');
    //    foreach (string key_ in keys)
    //    {
    //        Key.Add(key_);
    //    }

    //    //�� ������ �б� : value�� ����
    //    short LDCount = 0;
    //    short LCount = 0;
    //    while ((line = stream.ReadLine()) != null)
    //    {
    //        string[] values = line.Split(',');
    //        parsingDictionary.Add(new Dictionary<string, string>());
    //        foreach (string value_ in values)
    //        {
    //            parsingDictionary[LDCount].Add(Key[LCount++], value_);
    //        }
    //        LDCount++;
    //        LCount = 0;
    //    }
    //    //��ȯ
    //    return parsingDictionary;
    //}

    
    ///manual
    /*
        StringBuilder sb = new StringBuilder();
        sb.Append(Application.dataPath);
        sb.Append("/30.DataBase/DataJson/jTest1.json");
        ����ȭ : Json���� ��ȯ
        JsonTestClass jTest1 = new JsonTestClass();
        string jsonData = JsonConvert.SerializeObject(jTest1, Formatting.Indented);
        File.WriteAllText(sb.ToString(),jsonData);

        ������ȭ : ������Ʈ�� ��ȯ
        string jsonText;
        jsonText =   File.ReadAllText(sb.ToString());
        JsonTestClass jTest2 = JsonConvert.DeserializeObject<JsonTestClass>(jsonText);
        jTest2.Print();

        �̷��� vector3�� ����ȭ�ϸ� �������� normalized, maginute ��� �ٸ��͵鵵 ������ �ȴ�
        JsonVector vec = new JsonVector();
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        print(JsonConvert.SerializeObject(vec, settings));
    */


}
