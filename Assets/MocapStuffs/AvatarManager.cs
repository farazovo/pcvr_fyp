using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityVicon;

[RequireComponent(typeof(ViconDataStreamClient))]
public class AvatarManager : MonoBehaviour
{
    [SerializeField] string viconSubjectName;
    [SerializeField] GameObject avatarPersonal;
    [SerializeField] GameObject avatarGeneric;
    [Header("Debug")]
    [SerializeField] private bool debugDraw;
    [SerializeField, Min(0.001f)] private float debugDrawSphereRadius;
    [Header("Wiring")]
    [SerializeField] private Transform invisAvatarTm;

    public static AvatarManager Instance { get { return _instance; } }

    private ViconDataStreamClient _viconDataStreamClient;
    [HideInInspector]
    internal GameObject _avatarPersonal;
    private GameObject _avatarGeneric;
    private Transform _xrOriginTm;
    private Transform[] _avatarTransforms;
    private static AvatarManager _instance;

    public enum State
    {
        NoAvatar,
        Personalised,
        Generic
    }

    public void SetState(State state)
    {
        _avatarPersonal.SetActive(state == State.Personalised);
        _avatarGeneric.SetActive(state == State.Generic);
    }

    public Transform[] GetTransforms() { return _avatarTransforms; }

    void Awake()
    {        
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("Tried to created two AvatarManagers! Deleting");
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        _viconDataStreamClient = GetComponent<ViconDataStreamClient>();
        _xrOriginTm = GetComponentInChildren<XROrigin>().transform;
        Assert.IsNotNull(_xrOriginTm);

        _avatarTransforms = invisAvatarTm.GetChild(0).GetComponentsInChildren<Transform>();

        var ssHips = invisAvatarTm.GetChild(0).AddComponent<SubjectScript>();
        ssHips.Client = _viconDataStreamClient;
        ssHips.SubjectName = viconSubjectName;

        _avatarPersonal = Instantiate(avatarPersonal);
        Assert.IsNotNull(_avatarPersonal);
        _avatarPersonal.SetActive(false);
        _avatarPersonal.transform.SetParent(_xrOriginTm, false);
        _avatarPersonal.transform.localPosition = Vector3.zero;
        _avatarPersonal.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, 90f));
        var ssPersonal = _avatarPersonal.AddComponent<SubjectScript>();
        ssPersonal.Client = _viconDataStreamClient;
        ssPersonal.SubjectName = viconSubjectName;

        _avatarGeneric = Instantiate(avatarGeneric);
        Assert.IsNotNull(_avatarGeneric);
        _avatarGeneric.SetActive(false);
        _avatarGeneric.transform.SetParent(_xrOriginTm, false);
        _avatarGeneric.transform.localPosition = Vector3.zero;
        _avatarGeneric.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, 90f));
        var ssGeneric = _avatarGeneric.AddComponent<SubjectScript>();
        ssGeneric.Client = _viconDataStreamClient;
        ssGeneric.SubjectName = viconSubjectName;
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw)
            return;
        if (_avatarTransforms == null)
            return;
        Color prevColor = Gizmos.color;
        Gizmos.color = Color.magenta;        
        for (int i = 0; i < _avatarTransforms.Length; i++)        
            Gizmos.DrawSphere(_avatarTransforms[i].position, debugDrawSphereRadius);        
        Gizmos.color = prevColor;
    }
}
