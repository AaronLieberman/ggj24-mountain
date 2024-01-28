using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public LayerMask Mask = ~0;
    public Camera Camera;

    private Vector3? _lastHitMapPos = null;
    private Vector3 _offset;

    void ReturnHome()
    {
        transform.localPosition = Vector3.zero;
    }

    void Start()
    {
        ReturnHome();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ReturnHome();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("DOWN");
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask))
            {
                Debug.Log($"HIT:{hit.collider.gameObject.name}");
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

        if (_lastHitMapPos != null && Input.GetMouseButton(0))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask))
            {
                var newOffset = hit.point - _offset;

                transform.position = _lastHitMapPos.Value + newOffset;
            }
        }

    }
}
