using System.Collections;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoadManager
{
    private static string pathFolder = "./SavedData/";

    public static int CountNumberOfFilesInFolder()
    {
        int fCount = Directory.GetFiles(pathFolder, "*", SearchOption.TopDirectoryOnly).Length;
        return fCount;
    }

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

        ArrayList obj = GetAllElementToSave();
        ArrayList icons = GetAllIconsToSave();
        bool refereeDropped = IsRefereeDropped();
        ElementData data = new ElementData(obj, icons, name, category, author, difficulty, answer, reason, state, filename, refereeDropped);
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

    private static bool IsRefereeDropped()
    {
        GameObject contenitoreElementiInseriti = GameObject.Find("ElementiInseriti");
        for (int i = 0; i < contenitoreElementiInseriti.transform.childCount; ++i)
        {
            if (contenitoreElementiInseriti.transform.GetChild(i).GetComponent<Referee>())
                return true;
        }

        return false;
    }
    
    private static ArrayList GetAllElementToSave()
    {
        GameObject contenitoreElementiInseriti = GameObject.Find("ElementiInseriti");
        ArrayList elementiInseriti = new ArrayList();
        for(int i = 0; i < contenitoreElementiInseriti.transform.childCount; ++i){
            Transform child = contenitoreElementiInseriti.transform.GetChild(i);
            elementiInseriti.Add(child);
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

    public static void DeleteSimulation(string codice)
    {
        File.Delete(pathFolder + codice + ".fun");
    }
    
}
