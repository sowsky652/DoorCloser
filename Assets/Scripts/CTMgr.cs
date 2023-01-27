using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CTMgr : MonoBehaviour
{
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }


    Queue<Vector3> destinaition;
    NavMeshAgent agent;
    public Order order;
    public LineRenderer lineprefeb;
    LineRenderer line;
    Vector3 fixlastpos;
    public GameObject circle;
    public GameObject lastpos;
    private bool arrived = true;
    Vector3 curDestination;
    private bool isEditing = false;
    public float meshResolution;
    public float viewRadius;
    [Range(0, 360), Serialize]
    public float viewAngle;
    public LayerMask targetMask, obstacleMask;
    public bool Roatating { set; get; }
    public List<Transform> visibleTargets = new List<Transform>();

    Mesh viewMesh;
    public MeshFilter viewMeshFilter;

    private Order selectedOrder;

    void Start()
    {
        circle.SetActive(false);
        line = Instantiate(lineprefeb);
        line.SetPosition(0, transform.position);
        line.GetComponent<ObjectPath>().ct = this;
        agent = GetComponent<NavMeshAgent>();
        destinaition = new Queue<Vector3>();
        // lineBakedMesh = new Mesh();
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine(FindTargetsWithDelay(0.2f));
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;

            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
    void Update()
    {

        Move();
        MakelineMesh();
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }
    void MakelineMesh()
    {
        if (line.positionCount > 1)
        {
            Mesh lineBakedMesh = new Mesh(); //Create a new Mesh (Empty at the moment)
            line.BakeMesh(lineBakedMesh, true); //Bake the line mesh to our mesh variable
            line.GetComponentInChildren<MeshCollider>().sharedMesh = lineBakedMesh; //Set the baked mesh to the MeshCollider
            //line.GetComponent<MeshCollider>().convex = true; //You need it convex if the mesh have any kind of holes
            //line.GetComponent<MeshCollider>().isTrigger = true;
        }
    }
    private void OnDrawGizmos()
    {

        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, viewRadius);
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Handles.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Handles.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        Handles.color = Color.red;
        foreach (Transform visible in visibleTargets)
        {
            Handles.DrawLine(transform.position, visible.transform.position);
        }
    }

    public void ClearOrder()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Order");
       
        foreach(var GO in temp)
        {
            if (GO.GetComponent<Order>().ct == this)
            {
                Destroy(GO);
            }
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        // viewRadius를 반지름으로 한 원 영역 내 targetMask 레이어인 콜라이더를 모두 가져옴
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                // 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면 visibleTargets에 Add
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public void Resetorder()
    {
        //   orders.Clear();

        lastpos.SetActive(false);
        destinaition.Clear();
        arrived = true;
        curDestination = default;

    }

    public void MakeLastPos()
    {
        lastpos.SetActive(true);
        fixlastpos = destinaition.Last();
        lastpos.transform.position = destinaition.Last();
        lastpos.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
        isEditing = false;
    }

    private void Move()
    {

        if (lastpos != default && !isEditing)
        {
            lastpos.transform.position = fixlastpos;
        }
        else if (isEditing && lastpos.active)
        {
            lastpos.transform.position = destinaition.Last();
        }
            

        if (arrived && destinaition.Count > 0)
        { 
            curDestination = destinaition.Dequeue();
            agent.SetDestination(curDestination);
            arrived = false;
        }
        else if (Vector3.Distance(curDestination, transform.position) < 1f)
        {
            var array = destinaition.ToArray();

            line.positionCount = array.Length;
            line.SetPositions(array);
            arrived = true;
            curDestination = default;
        }

        if (arrived && destinaition.Count <= 0)
        {
            curDestination = default;
            lastpos.SetActive(false);

        }
    }

    public void SetRotate(Vector3 mousepos)
    {
        if (Roatating && Vector3.Distance(mousepos, transform.position) > 2f)
        {
            var dir = (mousepos - transform.position).magnitude;
            transform.LookAt(new Vector3(mousepos.x, mousepos.y + 0.1f, mousepos.z));
        }
    }

    public void AddRotateOnPath(Vector3 mousepos)
    {
        selectedOrder.rot = mousepos;
        Debug.Log("fj");
      
    }

    public void SetSeletedOrder(Order order)
    {
        selectedOrder = order;
    }

    public void MakeOrder(Vector3 meshpoint)
    {
        var temp=Instantiate(order, meshpoint, Quaternion.identity);
        temp.ct = this;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Order>().ct == this)
        {
            var temp = other.gameObject.GetComponent<Order>();
            if (temp.reload)
            {
            }
            if (temp.rot != default)
            {
                Vector3 dir = temp.rot - this.transform.position;

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10);
            }
            if (temp.flash)
            {

            }


            Destroy(other.gameObject);
        }
    }


    public bool DistanceFromLastdestinaition(Vector3 mousepos)
    {
        if (destinaition.Count == 0)
            return true;
        if (Vector3.Distance(destinaition.Last(), mousepos) > 0.5f || destinaition.Count == 0)
        {
            return true;
        }
        return false;
    }

    public void AddDestination(Vector3 mousepos)
    {
        var pos = new Vector3(mousepos.x, mousepos.y + 0.1f, mousepos.z);
        destinaition.Enqueue(pos);
        lastpos.transform.position = mousepos;

    }

    public void OnClick()
    {
        OnPoint();
        isEditing = true;
        circle.SetActive(true);
        lastpos.SetActive(true);

    }

    public void OnPoint()
    {
        lastpos.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        line.material.color = new Color(line.material.color.r, line.material.color.g, line.material.color.b, 255);
        line.SetWidth(0.2f, 0.2f);

    }

    public void ClickDisable()
    {
        circle.SetActive(false);
        if (lastpos.active)
        {
            lastpos.GetComponent<Image>().color = new Color(1, 1, 1, 0.36f);
            line.SetWidth(0.05f, 0.05f);
        }
    }

    public void SetPath()
    {

    }


}
