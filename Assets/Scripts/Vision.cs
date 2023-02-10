using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum EEnemy
{
    Enemy,
    Player,

}

public class Vision : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360), Serialize]
    public float viewAngle;
    public LayerMask targetMask, obstacleMask;
    public List<Transform> visibleTargets = new List<Transform>();
    private Shooter shooter;
    Mesh viewMesh;
    public MeshFilter viewMeshFilter;
    public float meshResolution;
    public string TargetLayout;
    public EEnemy myTarget;
    private string targeting;
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

    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        // viewRadius�� ���������� �� �� ���� �� targetMask ���̾��� �ݶ��̴��� ��� ������
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        List<GameObject> listofenemy = new List<GameObject>();
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // �÷��̾�� forward�� target�� �̷�� ���� ������ ���� �����
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                // Ÿ������ ���� ����ĳ��Ʈ�� obstacleMask�� �ɸ��� ������ visibleTargets�� Add
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    if (LayerMask.NameToLayer(myTarget.ToString()) == target.gameObject.layer && shooter.attackTarget == null)
                    {
                        listofenemy.Add(target.gameObject);
                    }
                }
            }
        }
        if (listofenemy.Count == 0)
        {
            shooter.attackTarget = null;
        }

        if (shooter.attackTarget == null)
        {
            if (listofenemy.Count == 0)
            {
                shooter.attackTarget = null;
            }
            else
            {
                float shortest = Vector3.Distance(transform.position, listofenemy[0].transform.position);
                shooter.attackTarget = listofenemy[0];
                foreach (var r in listofenemy)
                {
                    if (shortest > Vector3.Distance(transform.position, r.transform.position))
                    {
                        shooter.attackTarget = r;
                    }
                }
                if (myTarget == EEnemy.Player)
                {
                    LookingforCover();

                }
                shooter.OnShoot();
            }
        }


    }

    void LookingforCover()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, 5);
        List<Cover> coverlist = new List<Cover>();
        foreach (Collider coll in colls)
        {
            if (coll.GetComponent<Cover>() != null)
            {
                coverlist.Add(coll.gameObject.GetComponent<Cover>());
            }
        }
        if (coverlist.Count == 0)
        {
            return;
        }
        Cover nearCover = coverlist[0];

        foreach (Cover cover in coverlist)
        {
            if (Vector3.Distance(transform.position, nearCover.transform.position) > Vector3.Distance(transform.position, cover.transform.position))
            {
                nearCover = cover;
            }
        }

        transform.GetComponent<NavMeshAgent>().SetDestination(nearCover.FindCover(shooter.attackTarget));
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

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindTargetsWithDelay(0.2f));
        shooter = GetComponent<Shooter>();
        viewMesh = new Mesh();

        viewMesh.name = "View Mesh";
        if (viewMeshFilter != null)
            viewMeshFilter.mesh = viewMesh;

    }

    // Update is called once per frame
    void Update()
    {
        if (shooter.attackTarget != null)
        {
            transform.LookAt(shooter.attackTarget.gameObject.transform.position);
            //transform.rotation = Quaternion.EulerRotation(new Vector3(0, shooter.attackTarget.transform.position.y));
        }
    }
    void LateUpdate()
    {
        DrawFieldOfView();
    }

    //private void OnDrawGizmos()
    //{

    //    Handles.color = Color.white;
    //    Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, viewRadius);
    //    Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
    //    Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

    //    Handles.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
    //    Handles.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

    //    Handles.color = Color.red;
    //    foreach (Transform visible in visibleTargets)
    //    {
    //        if (visible != null)
    //            Handles.DrawLine(transform.position, visible.transform.position);
    //    }
    //}

    public void OnTriggerEnter(Collider other)
    {

        //if (other.gameObject.name == "Path(Clone)" && other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        //    return;
        //if (other.gameObject.GetComponent<Order>().ct == this)
        //{
        //    var temp = other.gameObject.GetComponent<Order>();
        //    if (temp.reload)
        //    {
        //        shooter.Reload();
        //    }
        //    if (temp.rot != default)
        //    {
        //        transform.LookAt(temp.rot);
        //        //Vector3 dir = temp.rot - this.transform.position;

        //        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10);
        //    }
        //    if (temp.flash)
        //    {

        //    }
        //    if (temp.swap)
        //    {
        //        shooter.Swap();
        //    }


        //    Destroy(other.gameObject);
        //}
    }
}
