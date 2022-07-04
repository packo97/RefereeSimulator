using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public static class SaveLoadManager
{
    private static string pathFolder = "./SavedData/";
    
    public static ArrayList GetFilesName()
    {
        ArrayList files = new ArrayList(Directory.GetFiles(pathFolder));
        return files;
    }
    
    public static bool SaveSimulation(string name, string category, string author, string difficulty, string answer, string reason, string state)
    {
        string filename = name + "_" + category + "_" + state + ".fun";
        if (File.Exists(pathFolder + filename))
            return false;
        
        string path = pathFolder + filename;
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        
        List<(Transform, int)> obj = GetAllElementToSave();
        ArrayList icons = GetAllIconsToSave();
        bool refereeDropped = GameObject.Find("Controller").GetComponent<PitchController>().IsRefereeDropped();
        ArrayList recording = GetAllRecordingToSave();
        ElementData data = new ElementData(obj, icons, name, category, author, difficulty, answer, reason, state, filename, refereeDropped, recording);
        
        formatter.Serialize(stream, data);
        stream.Close();
        return true;
    }

    public static ElementData LoadSimulation(string codice)
    {
        string path = pathFolder + codice + ".fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ElementData data = formatter.Deserialize(stream) as ElementData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("save file not found in " + path);
            return null;
        }
    }

    private static List<(Transform, int)> GetAllElementToSave()
    {
        GameObject contenitoreElementiInseriti = GameObject.Find("ElementiInseriti");
        List<(Transform, int)> elementiInseriti = new List<(Transform, int)>();
        for(int i = 0; i < contenitoreElementiInseriti.transform.childCount; ++i){
            Transform child = contenitoreElementiInseriti.transform.GetChild(i);
            int id = 0;
            if (contenitoreElementiInseriti.transform.GetChild(i).GetComponent<Player>())
                id = contenitoreElementiInseriti.transform.GetChild(i).GetComponent<Player>().id;
            elementiInseriti.Add((child, id));
            
        }
        

        return elementiInseriti;

    }
    private static ArrayList GetAllIconsToSave()
    {
        GameObject contenitoreIcone = null;
        GameObject editor = GameObject.Find("Editor");
        RectTransform[] children= editor.GetComponentsInChildren<RectTransform>(true);
        foreach (RectTransform child in children)
        {
            if (child.name.Equals("IconeInserite"))
                contenitoreIcone = child.gameObject;
        }
        
        ArrayList iconeInserite = new ArrayList();
        for(int i = 0; i < contenitoreIcone.transform.childCount; ++i){
            Transform child = contenitoreIcone.transform.GetChild(i);
            iconeInserite.Add(child);
            
        }
        

        return iconeInserite;

    }

    private static ArrayList GetAllRecordingToSave()
    {
        return GameObject.Find("Controller").GetComponent<ActionsController>().GetAllActionsRegistered();
    }

    public static void DeleteSimulation(string codice)
    {
        File.Delete(pathFolder + codice + ".fun");
    }
    
}
