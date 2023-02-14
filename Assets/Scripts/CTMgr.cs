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
    Queue<Vector3> destinaition;
    NavMeshAgent agent;
    float agentspeed;
    public Order order;
    public LineRenderer lineprefeb;
    LineRenderer line;
    Vector3 fixlastpos;
    public GameObject circle;
    public GameObject rotateCircle;
    public GameObject lastpos;
    public GameObject arrow;
    private Shooter shooter;
    private bool arrived = true;
    Vector3 curDestination;
    private bool isEditing = false;

    public bool Roatating { set; get; }

    private Order selectedOrder;
    Animator animator;
    Vector3 bookedrot;

    void Start()
    {
        circle.SetActive(false);
        line = Instantiate(lineprefeb);
        line.SetPosition(0, transform.position);
        line.GetComponent<ObjectPath>().ct = this;
        agent = GetComponent<NavMeshAgent>();
        destinaition = new Queue<Vector3>();

        agentspeed = agent.speed;

        animator = GetComponentInChildren<Animator>();

        shooter = GetComponent<Shooter>();
        GameManager.instance.CtAdd(this);
    }

    public Order GetSelectedOrder() { return selectedOrder; }

    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }


    void Update()
    {

        // if (!GameManager.instance.IsStop())
        Move();
        MakelineMesh();


    }


    public void Resume()
    {
        // animator.SetFloat("Speed", 1);
        SetRotate(bookedrot);
        bookedrot = default;
        agent.enabled = true;
        if (curDestination != default)
            agent.SetDestination(curDestination);
    }

    public void Stop()
    {
        animator.SetFloat("Speed", 0);
        agent.enabled = false;
    }

    public void RotatingValue(float value)
    {
        if (!rotateCircle.active)
        {
            rotateCircle.SetActive(true);
        }

        rotateCircle.GetComponent<Slider>().value = value;

    }


    public void BookedRotation(Vector3 mousepos)
    {
        var dir = (mousepos - transform.position).magnitude;
        bookedrot = new Vector3(mousepos.x, mousepos.y + 0.1f, mousepos.z);

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

    public void Dead()
    {
        if (line != null)
        {
            Destroy(line);
        }
        ClearOrder();
        GameManager.instance.CtRemove(this);
    }

    public void EditPath(Vector3 mousepos)
    {
        Vector3 near;

        int count = 0, index = 0;
        float min = 0;
        near = destinaition.First();
        foreach (var temp in destinaition)
        {
            if (Vector3.Distance(temp, mousepos) < Vector3.Distance(mousepos, near))
            {
                near = temp;
                index = count;
                min = Vector3.Distance(temp, mousepos);
            }
            ++count;
        }

        Queue<Vector3> newQ = new Queue<Vector3>();

        for (int i = 0; i < index; ++i)
        {
            newQ.Enqueue(destinaition.Dequeue());
        }
        destinaition = newQ;

        AddDestination(mousepos);
        lastpos.transform.position = mousepos;

        line.positionCount = destinaition.ToArray().Length;
        line.SetPositions(destinaition.ToArray());
    }

    public void ClearOrder()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Order");

        foreach (var GO in temp)
        {
            if (GO.GetComponent<Order>().ct == this)
            {
                Destroy(GO);
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
        if (destinaition.Count != 0)
        {
            lastpos.SetActive(true);
            fixlastpos = destinaition.Last();
            lastpos.transform.position = destinaition.Last();
            lastpos.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
            isEditing = false;
        }
    }

    private void Move()
    {

        if (lastpos != default && !isEditing)
        {
            lastpos.transform.position = fixlastpos;
        }
        else if (isEditing && destinaition.Count >= 1)
        {
            lastpos.transform.position = destinaition.Last();
        }


        if (arrived && destinaition.Count > 0)
        {
            curDestination = destinaition.Dequeue();
            agent.SetDestination(curDestination);
            arrived = false;
            animator.SetBool("isRunning", true);
            animator.SetLayerWeight(1, 1);

        }
        else if (Vector3.Distance(curDestination, transform.position) < 1f)
        {
            line.positionCount = destinaition.ToArray().Length;
            line.SetPositions(destinaition.ToArray());
            arrived = true;
            curDestination = default;

        }

        if (arrived && destinaition.Count <= 0)
        {
            curDestination = default;
            lastpos.SetActive(false);
            animator.SetBool("isRunning", false);
            animator.SetFloat("Speed", 0);
            animator.SetLayerWeight(1, 0);
        }
        else
        {
            animator.SetFloat("Speed", 1);
        }
    }

    public void SetRotate(Vector3 mousepos)
    {
        if (Vector3.Distance(mousepos, transform.position) > 2f)
        {
            transform.LookAt(new Vector3(mousepos.x, transform.position.y + 0.1f, mousepos.z));
          
            arrow.SetActive(true);
        }
    }

    public void AddRotateOnPath(Vector3 mousepos)
    {
        if (selectedOrder == null)
        {
            Debug.Log("selectedOrder is null");
        }
        if (selectedOrder != null)
            selectedOrder.rot = mousepos;
    }

    public void SetSeletedOrder(Order order)
    {
        selectedOrder = order;
    }

    public void MakeOrder(Vector3 meshpoint)
    {
        var temp = Instantiate(order, meshpoint, Quaternion.identity);
        temp.ct = this;
        selectedOrder = temp;
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
        line.positionCount = destinaition.ToArray().Length;
        line.SetPositions(destinaition.ToArray());
    }

    public void OnClick()
    {
        OnPoint();
        isEditing = true;

    }

    public void OnPoint()
    {
        lastpos.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        line.material.color = new Color(line.material.color.r, line.material.color.g, line.material.color.b, 255);
        line.SetWidth(0.2f, 0.2f);
        circle.SetActive(true);
    }

    public void ClickDisable()
    {
        circle.SetActive(false);
        if (lastpos.active)
        {
            lastpos.GetComponent<Image>().color = new Color(1, 1, 1, 0.36f);
          //  line.SetWidth(0.1f, 0.1f);
        }
        if (arrow.active)
        {
            arrow.SetActive(false);
        }
    }

    public void SetPath()
    {

    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name == "Path(Clone)" && other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            return;
        if (other.gameObject.GetComponent<Order>() != null && other.gameObject.GetComponent<Order>().ct == this)
        {
            var temp = other.gameObject.GetComponent<Order>();
            if (temp.reload)
            {
                shooter.Reload();
            }
            if (temp.rot != default)
            {
                transform.LookAt(temp.rot);

            }
            if (temp.flash)
            {

            }
            if (temp.swap)
            {
                shooter.Swap();
            }


            Destroy(other.gameObject);
        }
    }
}
