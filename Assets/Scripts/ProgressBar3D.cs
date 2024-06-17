using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar3D : MonoBehaviour
{

    [SerializeField] Transform segmentPrefab;
    [SerializeField] float length;
    [SerializeField] int nSegments;
    [SerializeField] float spacing;
    [SerializeField] float widthMultiplier=2; //x scale in relation to calculated segment height; -1 to not scale
    [SerializeField] float depthMultiplier=0.1f; //z scale in relation to calculated segment height; -1 to not scale
    // [SerializeField] float degrees;
    [SerializeField] float radius;
    // [Tooltip("Ignore multipliers and maintain relative scale of prefab.")] //TODO: Implment keep relative scale
    // [SerializeField] bool keepRelativeScale; // apply segment height to all axes; ignores width and depth settings;
    private float pctProgress = 1f;
    private int nActiveSegments;

    // Transform[] _segments;
    // Transform[] segments {
    //     get {
    //         if(_segments==null) { InitSegments(); } return _segments;
    //     }
    // }

    Transform[] segments;

    void Awake() {
        DeleteSegments();//Ensure prefab preview segments are destroyed
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSegments();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.T)) { //! For testings
            DeleteSegments();
            InitSegments();
        }
    }

    public void SetProgress(float pct) {
        if(segments==null) return;
        pctProgress=pct;
        nActiveSegments = (int)Mathf.Floor(pct*nSegments);
        for(int i=0; i<nSegments; i++) {
            segments[i].gameObject.SetActive(i<=nActiveSegments);
        }
    }

    public int GetNActiveSegments() {
        return nActiveSegments;
    }

    public Transform[] GetSegments() {
        if(segments!=null) {return segments;}
        InitSegments();
        return segments;
    }

    void DeleteSegments() {
        //Destroy existing children
        foreach(Transform _t in transform) {
            Destroy(_t.gameObject);
        }
        segments = null;
    }

    void InitSegments() {
        DeleteSegments();
        if(radius==0) {
            InitSegmentLine();
        } else {
            InitSegmentArc();
        }
    }

    void InitSegmentLine() {
        float totalSpacing = spacing*(nSegments-1);
        float effectiveLength = length-totalSpacing;
        float scale = effectiveLength/(nSegments-1);
        float incrementAmount= scale+spacing;
        Vector3 startPosition = Vector3.zero;
        Vector3 incrementAxis = new Vector3(0,1,0);
        Vector3 positionIncrement = incrementAxis * incrementAmount;
        Vector3 nextSegmentPosition = startPosition;
        List<Transform> newSegments = new List<Transform>();
        for(int i=0; i<nSegments;i++) {
            Transform newSegment = Instantiate(segmentPrefab, transform);
            newSegment.SetLocalPositionAndRotation(nextSegmentPosition, Quaternion.identity); // TODO: allow segment rotation property
            ScaleSegment(newSegment, scale);
            nextSegmentPosition +=positionIncrement;
            newSegments.Add(newSegment);
        }
        segments = newSegments.ToArray();
    }

void InitSegmentArc() {
    float totalSpacing = spacing * (nSegments - 1);
    float effectiveLength = length - totalSpacing;
    float segmentArcLength = effectiveLength / nSegments;
    float arcLength = effectiveLength + totalSpacing;
    float totalRadians = arcLength / radius;
    float angleIncrement = totalRadians / nSegments;
    float currentAngle = 0;

    Vector3 center = new Vector3(0, -radius, 0);

    List<Transform> newSegments = new List<Transform>();

    for (int i = 0; i < nSegments; i++) {
        Transform newSegment = Instantiate(segmentPrefab, transform);
        float angleRad = currentAngle;
        Vector3 position = new Vector3(Mathf.Cos(angleRad) * radius - radius, Mathf.Sin(angleRad) * radius, 0);
        Quaternion segmentRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * currentAngle);
        newSegment.SetLocalPositionAndRotation(position, segmentRotation);
        ScaleSegment(newSegment, segmentArcLength);
        currentAngle += angleIncrement;
        newSegments.Add(newSegment);
    }
    segments = newSegments.ToArray();
}

    void ScaleSegment(Transform newSegment, float segmentYScale) {
        // if(keepRelativeScale) {
        //     //TODO
        // }
        float x = widthMultiplier==-1 ? newSegment.localScale.x : segmentYScale*widthMultiplier;
        float z = depthMultiplier==-1 ? newSegment.localScale.z : segmentYScale*depthMultiplier;
        newSegment.localScale = new Vector3(x,segmentYScale, z);
    }

}


//    void InitSegmentArc() {
//         float totalSpacing = spacing * (nSegments - 1);
//         float effectiveLength = length - totalSpacing;
//         float segmentArcLength = effectiveLength / nSegments;
//         float angleIncrement = degrees / (nSegments - 1);
//         float radius = effectiveLength / (Mathf.Deg2Rad * degrees);
//         float currentAngle = 0;

//         for (int i = 0; i < nSegments; i++) {
//             Transform newSegment = Instantiate(segmentPrefab, transform);
//             float angleRad = Mathf.Deg2Rad * currentAngle;
//             Vector3 localPosition = new Vector3(
//                 Mathf.Sin(angleRad) * radius,
//                 Mathf.Cos(angleRad) * radius - radius,
//                 0
//             );
//             Quaternion segmentRotation = Quaternion.Euler(0, 0, currentAngle);
//             newSegment.SetLocalPositionAndRotation(localPosition, segmentRotation);
//             ScaleSegment(newSegment, segmentArcLength);
//             currentAngle += angleIncrement;
//             segments.Add(newSegment);
//         }
//     }s

    // void InitSegmentArc() {
    //     float totalSpacing = spacing * (nSegments - 1);
    //     float effectiveLength = length - totalSpacing;
    //     float segmentArcLength = effectiveLength / nSegments;
    //     float angleIncrement = degrees / (nSegments - 1);
    //     float radius = effectiveLength / (Mathf.Deg2Rad * degrees);
    //     float currentAngle = 0;
    //     Vector3 center = new Vector3(0, -radius, 0);

    //     for (int i = 0; i < nSegments; i++) {
    //         Transform newSegment = Instantiate(segmentPrefab, transform);
    //         float angleRad = Mathf.Deg2Rad * currentAngle;
    //         Vector3 position = new Vector3(Mathf.Cos(angleRad) * radius + radius,Mathf.Sin(angleRad) * radius, 0);//new Vector3(Mathf.Sin(angleRad) * radius, Mathf.Cos(angleRad) * radius + radius, 0);
    //         Quaternion segmentRotation = Quaternion.Euler(0, 0, currentAngle);
    //         newSegment.SetLocalPositionAndRotation(position, segmentRotation);
    //         ScaleSegment(newSegment, segmentArcLength);
    //         currentAngle += angleIncrement;
    //         segments.Add(newSegment);
    //     }
    // }
