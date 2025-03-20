using UnityEngine;
using System.Collections.Generic;

public class WebDrawing : MonoBehaviour
{
    public static WebDrawing Instance;

    public GameObject webPrefab;
    public GameObject startMarkerPrefab;
    public GameObject endMarkerPrefab;

    private GameObject currentWeb;
    private GameObject startMarker;
    private GameObject endMarker;
    private Collider2D webCollider;

    private Vector3 startPoint;
    private Vector3 finalEndPoint;
    private bool isDrawing = false;

    public float maxWebLength = 5f;
    public float webThickness = 0.1f;

    private static List<GameObject> activeWebs = new List<GameObject>(); // Track placed webs
    private static List<GameObject> activeNodes = new List<GameObject>(); // Track nodes
    public int maxWebsAllowed = 2; // Max number of webs allowed at once

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (ToolManager.Instance.currentTool != ToolManager.ToolMode.WebTool || activeWebs.Count >= maxWebsAllowed)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (activeWebs.Count >= maxWebsAllowed) return;

            startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPoint.z = 0;
            isDrawing = true;

            // Spawn web at start point
            currentWeb = Instantiate(webPrefab, startPoint, Quaternion.identity);
            currentWeb.transform.localScale = new Vector3(0.01f, webThickness, 1);

            // Disable collider while drawing
            webCollider = currentWeb.GetComponent<Collider2D>();
            if (webCollider != null)
            {
                webCollider.enabled = false;
            }

            // Spawn start marker
            startMarker = Instantiate(startMarkerPrefab, startPoint, Quaternion.identity);
            activeNodes.Add(startMarker);

            // Spawn end marker immediately
            endMarker = Instantiate(endMarkerPrefab, startPoint, Quaternion.identity);
            activeNodes.Add(endMarker);

            // Assign the nodes to the web's WebCollision component
            WebCollision webScript = currentWeb.GetComponent<WebCollision>();
            if (webScript != null)
            {
                webScript.startMarker = startMarker;
                webScript.endMarker = endMarker;
            }
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {
            Vector3 tempEndPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tempEndPoint.z = 0;

            if (Vector3.Distance(startPoint, tempEndPoint) > maxWebLength)
            {
                Vector3 direction = (tempEndPoint - startPoint).normalized;
                tempEndPoint = startPoint + (direction * maxWebLength);
            }

            finalEndPoint = tempEndPoint;
            UpdateWebPosition(startPoint, finalEndPoint);
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false;

            if (currentWeb != null)
            {
                if (webCollider != null)
                {
                    webCollider.enabled = true;
                }

                if (endMarker != null)
                {
                    endMarker.transform.position = GetAccurateEndMarkerPosition();
                    RotateMarker(endMarker, startPoint, finalEndPoint);
                }

                if (startMarker != null)
                {
                    RotateMarker(startMarker, startPoint, finalEndPoint);
                }

                activeWebs.Add(currentWeb);

                RemoveWebAfterDelay(currentWeb, startMarker, endMarker, 2f);
            }

            currentWeb = null;
            startMarker = null;
            endMarker = null;
        }
    }

    public void DestroyWeb(GameObject web, GameObject startNode, GameObject endNode)
    {
        if (web == null) return;

        activeWebs.Remove(web);
        activeNodes.Remove(startNode);
        activeNodes.Remove(endNode);

        Destroy(web);
        Destroy(startNode);
        Destroy(endNode);
    }

    void RemoveWebAfterDelay(GameObject web, GameObject startNode, GameObject endNode, float delay)
    {
        StartCoroutine(RemoveWebCoroutine(web, startNode, endNode, delay));
    }

    private System.Collections.IEnumerator RemoveWebCoroutine(GameObject web, GameObject startNode, GameObject endNode, float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyWeb(web, startNode, endNode);
    }

    void UpdateWebPosition(Vector3 start, Vector3 end)
    {
        if (currentWeb == null) return;

        float length = Vector3.Distance(start, end);

        currentWeb.transform.position = start;
        RotateMarker(currentWeb, start, end);
        float baseSpriteLength = 1f;
        currentWeb.transform.localScale = new Vector3(length / baseSpriteLength, webThickness, 1);

        if (startMarker != null)
        {
            startMarker.transform.position = start;
            RotateMarker(startMarker, start, end);
        }

        if (endMarker != null)
        {
            endMarker.transform.position = GetAccurateEndMarkerPosition();
            RotateMarker(endMarker, start, end);
        }
    }

    Vector3 GetAccurateEndMarkerPosition()
    {
        float webScale = currentWeb.transform.localScale.x;
        Vector3 direction = (finalEndPoint - startPoint).normalized;
        return startPoint + direction * webScale;
    }

    void RotateMarker(GameObject marker, Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        marker.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

























