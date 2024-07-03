using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using Unity.VisualScripting.FullSerializer;
using ExcelDataReader;
//TODO : 만약 폴더가 추가되면 해야할 것 2
public enum EFolderName
{
    none,
    Item
}
public static class ConvertJson<T> where T : class , new()
{
    
    static string Mainpath = Path.Combine(Application.dataPath, "Resources", "Data","Json");
    static string ResourcesPath = Path.Combine("Data","Json");
    static List<string> PathFolderList = new List<string>{ "Item" };//TODO : 만약 폴더가 추가되면 해야할 것 3 총 3개

    /// <summary>
    /// Json파일 만들기
    /// </summary>
    /// <param MonsterName="FolderName">폴더의 이름</param>
    /// <param MonsterName="fileName">파일의 이름</param>
    /// <param MonsterName="obj">클래스</param>
    public static void CreateJsonFile(EFolderName FolderName, string FileName, T obj)
    {
        if (!Directory.Exists(Mainpath)) Directory.CreateDirectory(Mainpath);
        if (!Directory.Exists(Path.Combine(Mainpath, PathFolderList[(int)FolderName - 1]))) 
            Directory.CreateDirectory(Path.Combine(Mainpath, PathFolderList[(int)FolderName - 1]));

            //예외처리1. obj가 null일 경우
            if (obj == null) { Debug.Log("ERROR : 클래스가 null입니다"); return; }
        if (FolderName == EFolderName.none) {Debug.Log("선택된 foldername이 없습니다"); return; }

        //만약 폴더가 없을경우
        
        
        //path 설정
        string curPath = Path.Combine(Mainpath, PathFolderList[(int)FolderName -1],FileName+".json");
        
        string jsonData = JsonConvert.SerializeObject(obj, Formatting.Indented);
        System.IO.File.WriteAllText(curPath.ToString(), jsonData);
    }

    /// <summary>
    /// Json파일 읽기
    /// 파일이 존재하지 않는다면 Null을 반환
    /// </summary>
    /// <param MonsterName="FolderName">폴더의 이름</param>
    /// <param MonsterName="FileName">파일의 이름</param>
    /// <returns></returns>
    public static T ReadJsonFile(EFolderName FolderName,string FileName) 
    {
        //예외처리1. FileName의 파일이없을 경우
        //if (FolderName == EFolderName.none) { Debug.Log("선택된 foldername이 없습니다"); return null; }
        //TextAsset jsonTextAsset = Resources.Load<TextAsset>("data");
        //string curPath = Path.Combine(Mainpath, PathFolderList[(int)FolderName - 1], FileName + ".json");
        string resourcespath = Path.Combine(ResourcesPath, PathFolderList[(int)FolderName - 1], FileName);
        //if (!System.IO.File.Exists(curPath)) { Debug.Log($"ERROR : {FileName}이라는 파일이 없습니다!"); return null; }



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

        //예외처리1. obj가 null일 경우
        if (FolderName == EFolderName.none) { Debug.Log("선택된 foldername이 없습니다"); return; }
        //path 설정
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
    //    //List dictionary 초기화
    //    List<Dictionary<string, string>> parsingDictionary = new List<Dictionary<string, string>>(); //반환할 자료구조
    //    List<string> Key = new List<string>(); //반복하며 작업할 key값 변수
    //    string curPath = Path.Combine(Mainpath_, FileName + ".csv");
    //    StreamReader stream = new StreamReader(curPath);

    //    //첫줄 읽기 : key값 저장
    //    string line = stream.ReadLine();
    //    string[] keys = line.Split(',');
    //    foreach (string key_ in keys)
    //    {
    //        Key.Add(key_);
    //    }

    //    //그 다음줄 읽기 : value값 저장
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
    //    //반환
    //    return parsingDictionary;
    //}

    
    ///manual
    /*
        StringBuilder sb = new StringBuilder();
        sb.Append(Application.dataPath);
        sb.Append("/30.DataBase/DataJson/jTest1.json");
        직렬화 : Json으로 변환
        JsonTestClass jTest1 = new JsonTestClass();
        string jsonData = JsonConvert.SerializeObject(jTest1, Formatting.Indented);
        File.WriteAllText(sb.ToString(),jsonData);

        역직렬화 : 오브젝트로 변환
        string jsonText;
        jsonText =   File.ReadAllText(sb.ToString());
        JsonTestClass jTest2 = JsonConvert.DeserializeObject<JsonTestClass>(jsonText);
        jTest2.Print();

        이렇게 vector3를 직렬화하면 쓸데없는 normalized, maginute 등등 다른것들도 저장이 된다
        JsonVector vec = new JsonVector();
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        print(JsonConvert.SerializeObject(vec, settings));
    */


}
