using UnityEngine;

public class K_ExperienceManager : ART_ExperienceManager
{
    string StudyCondition = "P/H";

    internal override string FileHeader()
    {
        var b = base.FileHeader();

        return b + "Condition,";
    }
    internal override string GetData()
    {
        var d = base.GetData();
        return d + $"{StudyCondition},";
    }
}
