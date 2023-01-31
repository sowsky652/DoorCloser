using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (clikedplayer != null && (!drag && !rotdrag))
        {
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.GetComponent<CTMgr>() != clikedplayer)
            {
                clikedplayer.ClickDisable();
                clikedplayer = null;
            }
        }

        if (Physics.Raycast(ray, out hit) && (hit.collider.transform.CompareTag("Player") || hit.collider.transform.CompareTag("Path") || hit.collider.transform.CompareTag("Order")))
        {
            if (!drag && !rotdrag)
            {
                if (clikedplayer != null)
                {
                    clikedplayer.ClickDisable();
                }

                if (hit.collider.transform.CompareTag("Player"))
                {
                    clikedplayer = hit.collider.gameObject.GetComponent<CTMgr>();
                }
                else if (hit.collider.transform.CompareTag("Path"))
                {
                    clikedplayer = hit.collider.gameObject.GetComponent<ObjectPath>().ct;
                }
                else if (hit.collider.transform.CompareTag("Order"))
                {
                    clikedplayer = hit.collider.gameObject.GetComponent<Order>().ct;
                }

                clikedplayer.OnClick();
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
                    clikedplayer.Roatating = true;
                    rotdrag = true;
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

            if (Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Order"))
            {
                if (Input.GetMouseButtonDown(1))
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
            }

            if (clikedplayer != null &&
                Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Floor"))
            {
                if (!GameManager.instance.IsStop() && rotdrag)
                    clikedplayer.SetRotate(hit.point);
                else
                    clikedplayer.BookedRotation(hit.point);
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (Physics.Raycast(ray, out hit) && hit.collider.transform.CompareTag("Floor"))
            {
                if (clikedplayer != null)
                {
                    clikedplayer.AddRotateOnPath(hit.point);
                    clikedplayer.ClickDisable();
                    clikedplayer = null;
                }
            }
            rotdrag = false;
            if (clikedplayer != null)
                clikedplayer.Roatating = false;

        }

        if (Input.GetMouseButtonUp(1) && clikedplayer != null)
        {
            if (clikedplayer.Roatating)
            {
                rotdrag = false;

                clikedplayer.Roatating = false;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
            {
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

                if (hit.collider.transform.CompareTag("Path"))
                {
                    clikedplayer.EditPath(hit.point);
                    
                     IncreasePath(clikedplayer.gameObject.transform.Find("LastPos").gameObject);
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
            if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.CompareTag("Floor") || hit.collider.gameObject.CompareTag("Path")))
            {

                drag = false;
                if (clikedplayer != null)
                {
                    clikedplayer.MakeLastPos();
                    clikedplayer.ClickDisable();
                }
            }
            else if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }

    }
}
