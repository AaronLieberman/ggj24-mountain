using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DragController : MonoBehaviour
{
    public LayerMask Mask = ~0;
    public MouseButton MouseButtonDrag = MouseButton.Left;
    public MouseButton MouseButtonReturnHome = MouseButton.Right;
    public Vector3 StartPos;

    public Camera Camera => Camera.main;
    public GameObject LinkedObj { get; private set; }

    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    private Vector3? _lastHitMapPos = null;
    private Vector3 _offset;

    private bool ShouldHandleMouseEvents
        => !EventSystem.current.IsPointerOverGameObject();

    void ReturnHome()
    {
        transform.localPosition = StartPos;
        LinkedObj.transform.position = transform.position;
    }

    void Start()
    {
        LinkedObj = Utilities.GetRootComponent<GameManager>().Map.gameObject;
        StartPos = LinkedObj.transform.position;
        transform.position = StartPos;
    }

    void Update()
    {
        if (!ShouldHandleMouseEvents)
        {
            _lastHitMapPos = null;
            transform.position = LinkedObj.transform.position;
            return;
        }

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
