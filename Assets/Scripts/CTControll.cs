using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTControll : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public GameObject UI;
    private CTMgr clikedplayer;
    private bool drag;
    private bool rotdrag;
    private float clickingtime = 0;
    private Vector3 clickedpostemp;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void IncreasePath(GameObject obj)
    {
        if (Input.GetMouseButton(1))
            return;
        drag = true;

        if (clikedplayer != null)
        {
            clikedplayer.ClickDisable();
        }

        obj.GetComponent<RectTransform>().localScale = Vector3.one;

        clikedplayer = obj.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CTMgr>();
        clikedplayer.OnClick();
    }

    // Update is called once per frame
    void Update()
    {
        clickingtime += Time.deltaTime;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray,out hit) && (hit.collider.transform.CompareTag("Player")||hit.collider.transform.CompareTag("Path")||hit.collider.transform.CompareTag("Order")))
        {
            if (hit.collider.transform.CompareTag("Order"))
            {

            }
        }
        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Player"))
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (clikedplayer != null)
                    {
                        clikedplayer.ClickDisable();
                    }
                    clikedplayer = hit.collider.gameObject.GetComponent<CTMgr>();
                    //  clikedplayer.OnClick();
                    clikedplayer.OnPoint();
                    clikedplayer.Roatating = true;
                }

            }

            if (Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Path"))
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (clikedplayer != null)
                    {
                        clikedplayer.ClickDisable();
                    }
                    clikedplayer = hit.collider.gameObject.GetComponent<ObjectPath>().ct;
                    clikedplayer.MakeOrder(hit.point);
                    
                }
            }

            if(Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Order"))
            {
                if (clikedplayer != null)
                {
                    clikedplayer.ClickDisable();
                }
                clikedplayer = hit.collider.gameObject.GetComponent<Order>().ct;
                clikedplayer.SetSeletedOrder(hit.collider.gameObject.GetComponent<Order>());
                clickedpostemp = hit.point;
                rotdrag = true;
            }

            if (clikedplayer != null &&
                Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Floor"))
            {
                clikedplayer.SetRotate(hit.point);
            }
        }

        if (rotdrag && Input.GetMouseButtonUp(1))
        {
            if (Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Floor"))
            {
                clikedplayer.AddRotateOnPath(hit.point);
                clikedplayer.ClickDisable();
                clikedplayer = null;
                rotdrag = false;
            }
        }

        if (Input.GetMouseButtonUp(1) && clikedplayer != null)
        {
            if (clikedplayer.Roatating)
            {
                clikedplayer.Roatating = false;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
            {
                clickingtime = 0;

                if (hit.collider.transform.CompareTag("Player"))
                {
                    if (clikedplayer != null)
                    {
                        clikedplayer.ClickDisable();
                    }
                    clikedplayer = hit.collider.gameObject.GetComponent<CTMgr>();
                    clikedplayer.Resetorder();
                    clikedplayer.OnClick();
                    clikedplayer.ClearOrder();
                    drag = true;

                }

            }


            if (drag)
            {
                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag("Floor"))
                {
                    if (clikedplayer.DistanceFromLastdestinaition(hit.point))
                    {
                        clikedplayer.AddDestination(hit.point);
                    }
                }
            }

        }

        if (drag && Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag("Floor"))
            {
                drag = false;
                clikedplayer.MakeLastPos();
            }
        }

        //if (Input.GetMouseButtonDown(1)&&)
        //{
        //}
    }
}
