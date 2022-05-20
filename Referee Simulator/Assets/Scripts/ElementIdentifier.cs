using UnityEngine;

[System.Serializable]
public class ElementIdentifier
{
    public int id;
    public string tag;

    public ElementIdentifier(int id, string tag)
    {
        this.id = id;
        this.tag = tag;
    }

    public override bool Equals(object obj)
    {
        Debug.Log("equals element identifier");
        ElementIdentifier ei = obj as ElementIdentifier;
        return id == ei.id && tag.Equals(ei.tag);
    }
}
