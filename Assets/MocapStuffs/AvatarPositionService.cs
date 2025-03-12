using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AvatarPositionService : BaseDevice
{
    public override string DeviceName()
    {
        return "VICON Avatar Position Service";
    }

    private Transform[] _avatarTransforms;

    internal override string FileHeader()
    {
        if(AvatarBase == null)
        {
            if (AvatarManager.Instance == null)
            {
                Debug.LogError("Unable to find an Avatar to Bind");

                return "";
            }
            AvatarBase = AvatarManager.Instance._avatarPersonal;
        }

        _avatarTransforms = AvatarBase.GetComponentsInChildren<Transform>(true);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < _avatarTransforms.Length; i++)
        {
            sb.Append($",Avatar_{_avatarTransforms[i].name}_pX,Avatar_{_avatarTransforms[i].name}_pY,Avatar_{_avatarTransforms[i].name}_pZ,Avatar_{_avatarTransforms[i].name}_qX,Avatar_{_avatarTransforms[i].name}_qY,Avatar_{_avatarTransforms[i].name}_qZ,Avatar_{_avatarTransforms[i].name}_qW");
        }

        return sb.ToString();
    }

    internal override string GetData()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < _avatarTransforms.Length; i++)
        {
            sb.Append($",{_avatarTransforms[i].position.x},{_avatarTransforms[i].position.y},{_avatarTransforms[i].position.z},{_avatarTransforms[i].rotation.x},{_avatarTransforms[i].rotation.y},{_avatarTransforms[i].rotation.z},{_avatarTransforms[i].rotation.w}");
        }

        return sb.ToString();
    }

    private GameObject AvatarBase = null;

    void Start()
    {
        if (AvatarManager.Instance == null)
            return;

        AvatarBase = AvatarManager.Instance._avatarPersonal;
    }
}
