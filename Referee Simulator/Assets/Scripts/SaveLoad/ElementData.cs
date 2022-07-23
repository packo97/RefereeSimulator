using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ElementData
{
    public string fileName;
    public bool refereeDropped;
    public List<Element> elements;
    public Details details;
    public ArrayList recording;
    
    public ElementData(List<(Transform, int)> obj, ArrayList icons, string name, string category, string author, string difficulty, string answer, string reason, string state, string fileName, bool refereeDropped, ArrayList recording)
    {
        elements = new List<Element>();
        for (int i = 0; i < obj.Count; i++)
        {
            RectTransform rt_icon = icons[i] as RectTransform;
            Transform t_element = rt_icon.GetComponent<DragDrop>().GetElementInThePitch().transform;
            int id_element = 0;
            bool isGoalKeeper = false;
            if (t_element.GetComponent<Player>())
            {
                id_element = t_element.GetComponent<Player>().id;
                isGoalKeeper = t_element.GetComponent<Player>().GetGoalKeeper();
            }
                
            //Element el = new Element(obj[i].Item1, icons[i] as RectTransform, obj[i].Item2);
            Element el = new Element(t_element, rt_icon, id_element, isGoalKeeper);

            elements.Add(el);
        }
        details = new Details(name, category, author, difficulty, answer, reason, state);
        this.fileName = fileName;
        this.refereeDropped = refereeDropped;

        this.recording = recording;
    }
    
    
    [System.Serializable]
    public class Element
    {
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public string type;

        public float iconPositionX;
        public float iconPositionY;
        public float iconPositionZ;
        public float iconRotationX;
        public float iconRotationY;
        public float iconRotationZ;

        public int id;
        public bool isGoalKeeper;
        
        public Element(Transform t, RectTransform rt, int id, bool isGoalKeeper)
        {
            //info elemento nel pitch
            positionX = t.transform.position.x;
            positionY = t.transform.position.y;
            positionZ = t.transform.position.z;
            rotationX = t.transform.eulerAngles.x;
            rotationY = t.transform.eulerAngles.y;
            rotationZ = t.transform.eulerAngles.z;
            type = t.tag;
            
            //info icona corrispondente
            iconPositionX = rt.position.x;
            iconPositionY = rt.position.y;
            iconPositionZ = rt.position.z;
            
            foreach (RectTransform i in rt.GetComponentsInChildren<RectTransform>())
            {
                if (i.name.Equals("Indicatore"))
                {
                    iconRotationX = i.eulerAngles.x;
                    iconRotationY = i.eulerAngles.y;
                    iconRotationZ = i.eulerAngles.z;
                }
                    
            }

            this.id = id;
            this.isGoalKeeper = isGoalKeeper;
        }
    }
    [System.Serializable]
    public class Details
    {
        public string name;
        public string category;
        public string author;
        public string difficulty;
        public string answer;
        public string reason;
        public string state;

        public Details(string name, string category, string author, string difficulty, string answer, string reason, string state)
        {
            this.name = name;
            this.category = category;
            this.author = author;
            this.difficulty = difficulty;
            this.answer = answer;
            this.reason = reason;
            this.state = state;
        }
    }
}