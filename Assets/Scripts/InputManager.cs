using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public LayerMask RaycastMask;

    private Highlight _currentHoverObject;

    [SerializeField]
    private Rain _rain;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Credits");
        }

        bool hitSomething = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,
            Mathf.Infinity, RaycastMask, QueryTriggerInteraction.Ignore);

        if (Input.GetMouseButtonDown(0))
        {
            if (_currentHoverObject != null)
            {
                _currentHoverObject.gameObject.SendMessage("OnClick");
            }
            else if (hitSomething && hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _rain.StartRain();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (hitSomething && hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _rain.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
            }
        }
        else if (hitSomething)
        {
            Highlight highlight = hit.transform.GetComponent<Highlight>();
            if (highlight != null)
            {
                RemoveHighlight();

                highlight.HighlightObject(true);
                _currentHoverObject = highlight;
            }
            else
            {
                RemoveHighlight();
            }
        }
        else
        {
            RemoveHighlight();
        }
    }

    void RemoveHighlight()
    {
        if (_currentHoverObject != null)
        {
            _currentHoverObject.HighlightObject(false);
            _currentHoverObject = null;
        }
    }
}