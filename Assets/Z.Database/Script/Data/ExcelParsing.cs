using ExcelDataReader;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ExcelParsing : MonoBehaviour
{
    public enum parsingtype
    {
        item,
        dialogue
    }
    public enum itemtype
    {
        equip,
        use,
        etc
    }

    [SerializeField] parsingtype parsetype;
    [SerializeField] itemtype item_type;

    private List<Dictionary<string, string>> excelparsescript(string path)
    {
        Debug.Log("파싱 시작");
        int t = (int)item_type;
        string filePath = Path.Combine(Application.dataPath, $"Z.Database/ExcelFile/{path}.xlsx");

        List<string> valueList = new List<string>();
        List<Dictionary<string, string>> Values = new List<Dictionary<string, string>>();

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                //시트 개수만큼 반복
                //해당 시트의 행데이터(한줄씩)로 반복
                for (int j = 0; j < result.Tables[t].Rows.Count; j++)
                {
                    if (j > 0)
                        Values.Add(new Dictionary<string, string>());
                    for (int k = 0; k < result.Tables[t].Columns.Count; k++)
                    {
                        //해당행의 0,1,2 셀의 데이터 파싱
                        string data = result.Tables[t].Rows[j][k].ToString();
                        if (j == 0)
                        {
                            valueList.Add(data);
                        }
                        else
                        {
                            Values[j - 1].Add(valueList[k], data);
                        }
                        Debug.Log(data);
                    }
                }
            }
        }
        return Values;
        //foreach( var value in Values)
        //{
        //    foreach( var data in value.Keys)
        //    {
        //        Debug.Log($"key : {data}  Value: {value[data]}");
        //    }
        //}
    }
    public void ItemCreateJsonfromExcel()
    {
        List<Dictionary<string, string>> value = excelparsescript("Data");

        int id = 0;
        string itemname = "", iconname = "", description = "";

        int sell = 0;
        int purchase = 0;
        switch (item_type)
        {
            case itemtype.equip:
                List<EquipItemInfo> equipinfo = new List<EquipItemInfo>();
                int str = 0, dex = 0, INT = 0, def = 0;
                int equiptype = 0;
                int Player_Class = 0;
                string GamePrefabName = "";
                foreach (var data in value)
                {
                    EquipItemInfo temp;
                    foreach (var g in data.Keys)
                    {
                        switch (g)
                        {
                            case "ID":
                                id = int.Parse(data[g]);
                                break;
                            case "NAME":
                                itemname = data[g];
                                break;
                            case "ICON":
                                iconname = data[g];
                                break;
                            case "DESCRIPTION":
                                description = data[g];
                                break;
                            case "STR":
                                str = int.Parse(data[g]);
                                break;
                            case "DEX":
                                dex = int.Parse(data[g]);
                                break;
                            case "INT":
                                INT = int.Parse(data[g]);
                                break;
                            case "DEF":
                                def = int.Parse(data[g]);
                                break;
                            case "EquipType":
                                equiptype = int.Parse(data[g]);
                                break;
                            case "PLAYER_CLASS":
                                Player_Class = int.Parse(data[g]);
                                break;
                            case "GamePrefabName":
                                GamePrefabName = data[g];
                                break;
                            case "sell":
                                sell = int.Parse(data[g]);
                                break;
                            case "purchase":
                                purchase = int.Parse(data[g]);
                                break;
                        }
                    }
                    temp = new EquipItemInfo(id, itemname, iconname, description,
                        str, dex, INT, def, equiptype, Player_Class, GamePrefabName,sell,purchase);
                    equipinfo.Add(temp);
                }
                ConvertJson<List<EquipItemInfo>>.CreateJsonFile(EFolderName.Item, "EquipItemsInfo", equipinfo);
                break;
            case itemtype.use:
                List<UseItemInfo> useinfo = new List<UseItemInfo>();
                int hp = 0, mp = 0; 
                foreach (var data in value)
                {

                    UseItemInfo temp;
                    foreach (var g in data.Keys)
                    {
                        switch (g)
                        {
                            case "ID":
                                id = int.Parse(data[g]);
                                break;
                            case "NAME":
                                itemname = data[g];
                                break;
                            case "ICON":
                                iconname = data[g];
                                break;
                            case "DESCRIPTION":
                                description = data[g];
                                break;
                            case "HP":
                                hp= int.Parse(data[g]);
                                break;
                            case "MP":
                                mp = int.Parse(data[g]);
                                break;
                            case "sell":
                                sell = int.Parse(data[g]);
                                break;
                            case "purchase":
                                purchase = int.Parse(data[g]);
                                break;
                        }
                    }
                    temp = new UseItemInfo(id, itemname, iconname, description,
                        hp, mp,sell, purchase);
                    useinfo.Add(temp);
                }

                ConvertJson<List<UseItemInfo>>.CreateJsonFile(EFolderName.Item, "UseItemsInfo", useinfo);
                break;
            case itemtype.etc:
                List<EtcItemInfo> etcinfo = new List<EtcItemInfo>();
                foreach (var data in value)
                {

                    EtcItemInfo temp;
                    foreach (var g in data.Keys)
                    {
                        switch (g)
                        {
                            case "ID":
                                id = int.Parse(data[g]);
                                break;
                            case "NAME":
                                itemname = data[g];
                                break;
                            case "ICON":
                                iconname = data[g];
                                break;
                            case "DESCRIPTION":
                                description = data[g];
                                break;
                            case "sell":
                                sell = int.Parse(data[g]);
                                break;
                            case "purchase":
                                purchase = int.Parse(data[g]);
                                break;
                        }
                    }
                    temp = new EtcItemInfo(id, itemname, iconname, description, sell, purchase);
                    etcinfo.Add(temp);
                }
                ConvertJson<List<EtcItemInfo>>.CreateJsonFile(EFolderName.Item, "EtcItemsInfo", etcinfo);
                break;
        }
    }
    StringBuilder str = new StringBuilder();
    int xorpassword = 12;
    public void DialogueCreateJsonfromExcel()
    {
        List<Dictionary<string, string>> value = excelparsescript("NpcTalk");

        int id = -1;
        string npcname = "";
        string talk = ""; 
        List<string> dialoglist = new List<string>();
        Dictionary<int, TalkDialogue> dialogue = new Dictionary<int, TalkDialogue>();
        TalkDialogue temp = null;
        //  id가 그전에거와 다르면 새로운 Talkdialogue 저장
        foreach (var data in value)
        {
            foreach (var g in data.Keys)
            {
                switch (g) //id,npcname,talk의 데이터 한줄을 불러오는 과정
                {
                    case "ID":
                        if (data[g] != "")
                        {
                            id = int.Parse(data[g]);
                            if (id != 0)
                            {
                                temp.Dialog = dialoglist;
                                dialogue.Add(temp.ID, temp);
                            }
                            temp = new TalkDialogue();
                            dialoglist = new List<string>();

                            temp.ID = id;
                        }
                        break;
                    case "NpcName":
                        if (data[g] != "")
                        {
                            npcname = data[g];
                            temp.NPCName = npcname;
                        }
                        break;
                    case "Dialog":
                        talk = data[g];
                        break;
                }
            }
            dialoglist.Add(talk);
        }
        temp.Dialog = dialoglist;
        dialogue.Add(temp.ID, temp);
        ConvertJson<Dictionary<int, TalkDialogue>>.CreateJsonFile(EFolderName.Item, "Dialogue", dialogue);

        
        //var path = AssetDatabase.GetAssetPath(xlsx);
        //var streamer = new FileStream(path, FileMode.Open, FileAccess.Read);

        //using (var reader = ExcelReaderFactory.CreateReader(streamer))
        //{
        //    // 모든 시트 로드
        //    var tables = reader.AsDataSet().Tables;
        //    for (var sheetIndex = 0; sheetIndex < tables.Count; sheetIndex++)
        //    {
        //        var sheet = tables[sheetIndex];

        //        //시트 이름 필터링 가능
        //        Debug.Log($"Sheet[{sheetIndex}] Name: {sheet.TableName}");

        //        for (var rowIndex = 0; rowIndex < sheet.Rows.Count; rowIndex++)
        //        {
        //            // 행 가져오기
        //            var slot = sheet.Rows[rowIndex];
        //            for (var columnIndex = 0; columnIndex < slot.ItemArray.Length; columnIndex++)
        //            {
        //                var item = slot.ItemArray[columnIndex];
        //                // 열 가져오기
        //                Debug.Log($"slot[{rowIndex}][{columnIndex}] : {item}");
        //            }
        //        }
        //    }
        //    reader.Dispose();
        //    reader.Close();
        //}
    }
}




