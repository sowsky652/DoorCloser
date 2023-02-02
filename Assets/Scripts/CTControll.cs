using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UIElements;

public class CTControll : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public GameObject UI;
    private CTMgr clikedplayer;
    private bool drag;
    private bool rotdrag;
    private Vector3 tempPos;
    private bool orderrot = false;
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
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit))
        {

            if (clikedplayer != null && (!drag))
            {
                if (hit.collider.GetComponent<CTMgr>() != null && hit.collider.GetComponent<CTMgr>() != clikedplayer)
                {
                    clikedplayer.ClickDisable();
                    clikedplayer = null;
                }
            }
            else if (clikedplayer == null && (hit.collider.transform.tag == "Player" || hit.collider.transform.tag == "Path"))
            {
                if (hit.collider.transform.tag == "Player")
                    clikedplayer = hit.collider.transform.GetComponent<CTMgr>();
                else if (hit.collider.transform.tag == "Path")
                    clikedplayer = hit.collider.transform.GetComponent<ObjectPath>().ct;
                //else if(hit.collider.transform.tag == "LastPos")
                //    clikedplayer = hit.collider.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CTMgr>(); 
                clikedplayer.OnPoint();
            }

            ////////////////////////////////////////////////////Leftclick
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.transform.tag == "Player")
                    {
                        if (clikedplayer != null)
                        {
                            clikedplayer.ClickDisable();
                        }
                        clikedplayer = hit.collider.transform.GetComponent<CTMgr>();
                        clikedplayer.Resetorder();
                        clikedplayer.ClearOrder();
                        clikedplayer.OnClick();
                        drag = true;
                    }
                    else if (hit.collider.transform.tag == "Path")
                    {
                        if (clikedplayer != null)
                        {
                            clikedplayer.ClickDisable();
                        }
                        clikedplayer = hit.collider.transform.GetComponent<ObjectPath>().ct;
                        clikedplayer.OnClick();
                        clikedplayer.EditPath(hit.point);
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
            if (Input.GetMouseButtonUp(0))
            {
                if (clikedplayer != null)
                {
                    clikedplayer.MakeLastPos();
                    clikedplayer.ClickDisable();
                    clikedplayer = null;
                }
                drag = false;
            }

            /////////////////////////////rightclick
            if (Input.GetMouseButton(1))
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (hit.collider.transform.tag == "Player")
                    {

                        if (clikedplayer != null)
                        {
                            clikedplayer.ClickDisable();
                        }
                        clikedplayer = hit.collider.transform.GetComponent<CTMgr>();
                        clikedplayer.OnClick();
                        clikedplayer.Roatating = true;
                        rotdrag = true;
                    }
                    else if (hit.collider.transform.tag == "Path")
                    {
                        if (hit.collider.transform.tag == "Path")
                        {

                            if (clikedplayer != null)
                                clikedplayer.MakeOrder(hit.point);
                        }
                        tempPos = hit.point;
                        orderrot = true;
                    }

                }
                if (rotdrag)
                {
                    if (clikedplayer != null)
                        clikedplayer.SetRotate(hit.point);
                }
            }
            if (Input.GetMouseButtonUp(1))
            {

                //if (hit.collider.transform.tag == "Player")
                //{

                //    if (clikedplayer != null)
                //    {
                //        clikedplayer.transform.Find("PlayerMenu").
                //    }
                        
                //}
                //else 
                if (hit.collider.transform.tag == "Order")
                {
                    hit.collider.transform.GetComponent<Order>().transform.Find("OrderMenu").gameObject.SetActive(true);
                }
                else if (orderrot&&hit.collider.transform.tag == "Floor")
                {
                    if (clikedplayer != null)
                        clikedplayer.MakeOrder(tempPos);
                    clikedplayer.AddRotateOnPath(hit.point);
                    tempPos = default;
                    orderrot= false;
                }
                rotdrag = false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////


        //if (clikedplayer != null && (!drag && !rotdrag))
        //{
        //    if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.GetComponent<CTMgr>() != clikedplayer)
        //    {
        //        clikedplayer.ClickDisable();
        //        clikedplayer = null;
        //    }
        //}

        //if (Physics.Raycast(ray, out hit) && (hit.collider.transform.CompareTag("Player") || hit.collider.transform.CompareTag("Path") || hit.collider.transform.CompareTag("Order")))
        //{
        //    if (!drag && !rotdrag)
        //    {
        //        if (clikedplayer != null)
        //        {
        //            clikedplayer.ClickDisable();
        //        }

        //        if (hit.collider.transform.CompareTag("Player"))
        //        {
        //            clikedplayer = hit.collider.gameObject.GetComponent<CTMgr>();
        //        }
        //        else if (hit.collider.transform.CompareTag("Path"))
        //        {
        //            clikedplayer = hit.collider.gameObject.GetComponent<ObjectPath>().ct;
        //        }
        //        else if (hit.collider.transform.CompareTag("Order"))
        //        {
        //            clikedplayer = hit.collider.gameObject.GetComponent<Order>().ct;
        //        }

        //        clikedplayer.OnClick();
        //    }

        //}
        //if (Input.GetMouseButton(1))
        //{
        //    if (Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Player"))
        //    {
        //        if (Input.GetMouseButtonDown(1))
        //        {
        //            if (clikedplayer != null)
        //            {
        //                clikedplayer.ClickDisable();
        //            }
        //            clikedplayer = hit.collider.gameObject.GetComponent<CTMgr>();
        //            clikedplayer.Roatating = true;
        //            rotdrag = true;
        //        }

        //    }

        //    if (Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Order"))
        //    {
        //        if (Input.GetMouseButtonDown(1))
        //        {
        //            if (clikedplayer != null)
        //            {
        //                clikedplayer.ClickDisable();
        //            }
        //            clikedplayer = hit.collider.gameObject.GetComponent<Order>().ct;
        //            clikedplayer.SetSeletedOrder(hit.collider.gameObject.GetComponent<Order>());
        //            clickedpostemp = hit.point;
        //            rotdrag = true;
        //        }
        //    }

        //    if (clikedplayer != null &&
        //        Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Floor"))
        //    {
        //        if (!GameManager.instance.IsStop() && rotdrag)
        //            clikedplayer.SetRotate(hit.point);
        //        else
        //            clikedplayer.BookedRotation(hit.point);
        //    }
        //}

        //if (Input.GetMouseButtonUp(1))
        //{
        //    if (Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Floor"))
        //    {
        //        if (clikedplayer != null)
        //        {
        //            clikedplayer.AddRotateOnPath(hit.point);
        //            clikedplayer.ClickDisable();
        //            clikedplayer = null;
        //        }
        //    }
        //    else if (Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Path"))
        //    {
        //        if (clikedplayer != null)
        //        {
        //            clikedplayer.ClickDisable();
        //        }
        //        clikedplayer = hit.collider.gameObject.GetComponent<ObjectPath>().ct;
        //        clikedplayer.MakeOrder(hit.point);

        //    }

        //    rotdrag = false;
        //    if (clikedplayer != null)
        //        clikedplayer.Roatating = false;

        //}

        //if (Input.GetMouseButtonUp(1) && clikedplayer != null)
        //{
        //    if (clikedplayer.Roatating)
        //    {
        //        rotdrag = false;

        //        clikedplayer.Roatating = false;
        //    }
        //}

        //if (Input.GetMouseButton(0))
        //{
        //    if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider.transform.CompareTag("Player"))
        //        {
        //            if (clikedplayer != null)
        //            {
        //                clikedplayer.ClickDisable();
        //            }
        //            clikedplayer = hit.collider.gameObject.GetComponent<CTMgr>();
        //            clikedplayer.Resetorder();
        //            clikedplayer.OnClick();
        //            clikedplayer.ClearOrder();
        //            drag = true;

        //        }

        //        if (hit.collider.transform.CompareTag("Path"))
        //        {
        //            clikedplayer.EditPath(hit.point);

        //            IncreasePath(clikedplayer.gameObject.transform.Find("LastPos").gameObject);
        //            drag = true;
        //        }

        //    }


        //    if (drag)
        //    {
        //        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag("Floor"))
        //        {
        //            if (clikedplayer.DistanceFromLastdestinaition(hit.point))
        //            {
        //                clikedplayer.AddDestination(hit.point);
        //            }
        //        }
        //    }

        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.CompareTag("Floor") || hit.collider.gameObject.CompareTag("Path")))
        //    {

        //        drag = false;
        //        if (clikedplayer != null)
        //        {
        //            clikedplayer.MakeLastPos();
        //            clikedplayer.ClickDisable();
        //        }
        //    }
        //    else if (Physics.Raycast(ray, out hit))
        //    {
        //        Debug.Log(hit.collider.gameObject.name);
        //    }
        //}

    }
}
