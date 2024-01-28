using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DragController : MonoBehaviour
{
    public LayerMask Mask = ~0;
    public Camera Camera;
    public GameObject LinkedObj;
    public MouseButton MouseButtonDrag = MouseButton.Middle;
    public MouseButton MouseButtonReturnHome = MouseButton.Right;
    public Vector3 StartPos;

    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    private Vector3? _lastHitMapPos = null;
    private Vector3 _offset;

    void ReturnHome()
    {
        transform.localPosition = StartPos;
        LinkedObj.transform.position = transform.position;
    }

    void Start()
    {
        StartPos = LinkedObj.transform.position;
        transform.position = StartPos;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown((int)MouseButtonReturnHome))
        {
            ReturnHome();
            return;
        }

        if (Input.GetMouseButtonDown((int)MouseButtonDrag))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    _lastHitMapPos = transform.position;
                    _offset = hit.point;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _lastHitMapPos = null;
            return;
        }

        if (_lastHitMapPos != null && Input.GetMouseButton((int)MouseButtonDrag))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask))
            {
                var newOffset = hit.point - _offset;

                transform.position = _lastHitMapPos.Value + newOffset;
            }
        }

        LinkedObj.transform.position = transform.position;
    }
}
