[System.Serializable]
public class TargetForElement
{
    public Vector3Surrogate target;
    public ElementIdentifier elementIdentifier;
    public bool alreadyUsed;

    public TargetForElement(Vector3Surrogate target, ElementIdentifier elementIdentifier)
    {
        this.target = target;
        this.elementIdentifier = elementIdentifier;
        alreadyUsed = false;
    }

    
}
