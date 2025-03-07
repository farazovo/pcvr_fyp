using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ViveSR.anipal.Eye;

public class AOITagger : BaseDevice
{
    public int ItemsToTag = 10;

    [SerializeAs("Layer To Tag"), SerializeField] private LayerMask _mask;

    private GazePixelAnalyser _tracker;
    // Start is called before the first frame update
    void Start()
    {
        _tracker = GetComponent<GazePixelAnalyser>();
        if (_tracker is null) Destroy(this);

        _hits = new RaycastHit[ItemsToTag];
    }

    [SerializeField] private string _lastTagged = "";
    private int lastHitCount = 0;

    // Update is called once per frame

    private void Update()
    {
        Debug.DrawRay(_ray.origin, _ray.direction * 10.0f, Color.green);
    }

    void FixedUpdate()
    {
        _lastTagged = DoTagging();
    }

    private RaycastHit[] _hits = new RaycastHit[10];
    private Ray _ray;

    private string DoTagging()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
            SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return "SRanipal Off";

        _ray = new Ray(Camera.main.transform.position, _tracker.GazeDirectionCombined);

        lastHitCount = Physics.RaycastNonAlloc(_ray, _hits, Mathf.Infinity, _mask);

        _hits = _hits.OrderBy(x => x.distance).ToArray();

        if (lastHitCount > 0)       
            if (_hits[0].transform != null)
                return _hits[0].transform.gameObject.tag;                    

        return "Nothing In Focus";
    }

    internal override string FileHeader()
    {
        string header = "";
        for (int i = 1; i <= _hits.Length; i++)
        {
            header += $",AIOTagged_{i}";
        }
        return header;
    }

    internal override string GetData()
    {
        string data = "";
        try
        {
            for (int i = 0; i < lastHitCount; i++)
                data += $",{_hits[i].transform.gameObject.tag}";
            for (int i = lastHitCount; i < _hits.Length; i++)
                data += ",";
        }
        catch
        {
            data = "";
            for (int i = 0; i < _hits.Length; i++)
                data += ",";
        }

        return data;
    }

    public override string DeviceName()
    {
        return "AIO Tagger";
    }

    public string getLastTagged() { return _lastTagged; }
}
