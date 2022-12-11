using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager
{
    public static string savePath = Application.persistentDataPath + "/saves/save.bnk";
    private static bool hasLoaded = false;

    public static bool HasSave()
    {
        if(File.Exists(savePath))
        {
            if (hasLoaded)
                return true;

            SaveData.current = (SaveData)Load(savePath);
            if (SaveData.current != null)
                hasLoaded = true;
            return SaveData.current != null;
        }

        return false;
    }

    public static void ForceLoad()
    {
        SaveData.current = (SaveData)Load(savePath);
    }

    public static bool Save(object _data)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if(!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        FileStream file = File.Create(savePath);
        formatter.Serialize(file, _data);
        file.Close();

        return true;
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        SaveData.current = null;
    }

    public static object Load(string _path)
    {
        if(!File.Exists(_path))
        {
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(_path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            Debug.Log("Succesful load");
            return save;
        }
        catch(Exception _e)
        {
            Debug.LogError(_e?.Message);
            Debug.LogError(_e.ToString());
            file.Close();
            return null;
        }
        catch
        {
            Debug.LogError("Failed to load file at " + _path);
            // Debug.Log(_ex.Message);
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        Vector3SerializationSurrogate v3Surrogate = new Vector3SerializationSurrogate();
        QuaternionSerializationSurrogate qSurrogate = new QuaternionSerializationSurrogate();
        ColorSerializationSurrogate cSurrogate = new ColorSerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), qSurrogate);
        selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), cSurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }
}
