using UnityEngine;

public class ReactTime : ART_AdditionalData
{
    float reactionTime = 0.0f;
    bool isReacting = true;

    internal override string FileHeader()
    {
        return "ReactionTime,IsReacting,";
    }

    internal override string GetData()
    {
       return $"{reactionTime},{isReacting},";
    }

/*    private void Update()
    {
        if (isReacting)
        {
            reactionTime += Time.deltaTime;
        }
        else
        {
            reactionTime = 0.0f;
        }
    }*/
}
