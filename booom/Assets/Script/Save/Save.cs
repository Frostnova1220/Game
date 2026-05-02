using System.IO;                
using UnityEngine;               
public class Save
{
    public static void SaveDate(string Name, int Age, int Height)
    {
        var data = new PlayerData();

        // 把 PlayerData 对象转换为 JSON 字符串
        var jsonStr = JsonUtility.ToJson(data);
        // 设定文件存储的路径

        var filePath = Application.persistentDataPath + "/PlayerData.json";
        System.IO.File.WriteAllText(filePath, jsonStr);
    }

    // 静态方法：从 JSON 文件加载数据，返回 PlayerData 对象
    public static PlayerData LoadDate()
    {
        // 构建同样的文件路径
        var filePath = Application.persistentDataPath + "/PlayerData.json";

        if (File.Exists(filePath))
        {

            var jsonStr = System.IO.File.ReadAllText(filePath);
            var date = JsonUtility.FromJson<PlayerData>(jsonStr);
            return date; // 返回加载的对象
        }
        else
        {
            return new PlayerData();
        }
    }
}

// 可序列化的数据类，用于存储玩家数据
public class PlayerData
{
}