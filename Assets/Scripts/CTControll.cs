using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CTControll : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    private CTMgr clikedplayer;
    private bool drag;
    private bool rotdrag;
    private Vector3 tempPos;
    private bool rotating = false;
    private bool playerclick = false;
    private bool pathclick = false;

    private float clikedTime = 0f;

    Vector2 prevtouch;

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
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            ray = Camera.main.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out hit))
            {

                //if (clikedplayer != null && (!drag))
                //{
                //    if (hit.collider.GetComponent<CTMgr>() != null && hit.collider.GetComponent<CTMgr>() != clikedplayer)
                //    {
                //        clikedplayer.ClickDisable();
                //        clikedplayer = null;
                //    }
                //}
                //else if (clikedplayer == null && (hit.collider.transform.tag == "Player" || hit.collider.transform.tag == "Path"))
                //{
                //    if (hit.collider.transform.tag == "Player")
                //        clikedplayer = hit.collider.transform.GetComponent<CTMgr>();
                //    else if (hit.collider.transform.tag == "Path")
                //        clikedplayer = hit.collider.transform.GetComponent<ObjectPath>().ct;
                //    //else if(hit.collider.transform.tag == "LastPos")
                //    //    clikedplayer = hit.collider.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CTMgr>(); 
                //    clikedplayer.OnPoint();
                //}

                ////////////////////////////////////////////////////Leftclick


                if (clikedplayer == null && hit.collider.transform.tag == "Player")
                {
                    clikedplayer = hit.collider.transform.GetComponent<CTMgr>();
                    clikedplayer.Resetorder();
                    clikedplayer.ClearOrder();
                    clikedplayer.OnClick();
                    playerclick = true;
                    prevtouch = touch.position;
                }
                else if (clikedplayer == null && hit.collider.transform.tag == "Path")
                {
                    clikedplayer = hit.collider.transform.GetComponent<ObjectPath>().ct;
                    clikedplayer.OnClick();
                    clikedplayer.EditPath(hit.point);
                    pathclick = true;
                    prevtouch = touch.position;

                }

                if (prevtouch != touch.position)
                {
                    if (playerclick)
                        drag = true;        
                        
                }
                else if (prevtouch == touch.position)
                {
                    clikedTime += Time.deltaTime;
                    if (clikedTime >= 0.6f)
                    {
                        rotating = true;
                    }
                }

                //if (playerclick && (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0))
                //{
                //    clikedTime += Time.deltaTime;
                //    if (clikedTime >= 0.6f)
                //    {
                //        rotating = true;

                //    }
                //}
                //else if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                //{
                //    drag = true;
                //    clikedTime = 0;
                //}

                if (rotating)
                {
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        if (clikedplayer != null)
                        {
                            clikedplayer.SetRotate(hit.point);
                        }
                    }
                }
                else if (drag)
                {
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        if (clikedplayer.DistanceFromLastdestinaition(hit.point))
                        {
                            clikedplayer.AddDestination(hit.point);
                        }
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if (clikedplayer != null)
                {
                    clikedplayer.MakeLastPos();
                    clikedplayer.ClickDisable();
                    clikedplayer = null;
                }

                drag = false;
                rotating = false;
                pathclick = false;
                clikedTime = 0;
            }
            /////////////////////////////rightclick
            //if (Input.GetMouseButton(1))
            //{
            //    if (Input.GetMouseButtonDown(1))
            //    {
            //        if (hit.collider.transform.tag == "Player")
            //        {

            //            if (clikedplayer != null)
            //            {
            //                clikedplayer.ClickDisable();
            //            }
            //            clikedplayer = hit.collider.transform.GetComponent<CTMgr>();
            //            clikedplayer.OnClick();
            //            clikedplayer.Roatating = true;
            //            rotdrag = true;
            //        }
            //        else if (hit.collider.transform.tag == "Path")
            //        {
            //            if (hit.collider.transform.tag == "Path")
            //            {

            //                if (clikedplayer != null)
            //                    clikedplayer.MakeOrder(hit.point);
            //            }
            //            tempPos = hit.point;
            //            orderrot = true;
            //        }

            //    }
            //    if (rotdrag)
            //    {
            //        if (clikedplayer != null)
            //            clikedplayer.SetRotate(hit.point);
            //    }
            //}
            //if (Input.GetMouseButtonUp(1))
            //{

            //    if (hit.collider.transform.tag == "Order")
            //    {
            //        hit.collider.transform.GetComponent<Order>().transform.Find("OrderMenu").gameObject.SetActive(true);
            //    }
            //    else if (orderrot && hit.collider.transform.tag == "Floor")
            //    {
            //        if (clikedplayer != null)
            //            clikedplayer.MakeOrder(tempPos);
            //        clikedplayer.AddRotateOnPath(hit.point);
            //        tempPos = default;
            //        orderrot = false;
            //    }
            //    rotdrag = false;
            //}
        }

    }
}
