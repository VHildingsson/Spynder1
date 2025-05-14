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

    private List<GameObject> activeWebs = new List<GameObject>();
    private List<GameObject> activeNodes = new List<GameObject>();
    public int maxWebsAllowed = 2;

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

    private void Update()
    {
        if (isDrawing && currentWeb != null)
        {
            UpdateCurrentWeb();
        }
    }

    public bool CanStartNewWeb()
    {
        return ToolManager.Instance.currentTool == ToolManager.ToolMode.WebTool;
    }

    public void StartPotentialWeb()
    {
        if (!CanStartNewWeb()) return;

        // Remove oldest web if we're at max capacity
        if (activeWebs.Count >= maxWebsAllowed)
        {
            RemoveOldestWeb();
        }

        startPoint = CursorManager.Instance.GetCursorWorldPosition();
        startPoint.z = 0;
        isDrawing = true;

        CreateWebObjects();
        SetupWebCollision();
    }

    private void RemoveOldestWeb()
    {
        if (activeWebs.Count == 0) return;

        // Get the oldest web (first in list)
        GameObject oldestWeb = activeWebs[0];
        GameObject startNode = null;
        GameObject endNode = null;

        // Find associated nodes
        WebCollision webScript = oldestWeb.GetComponent<WebCollision>();
        if (webScript != null)
        {
            startNode = webScript.startMarker;
            endNode = webScript.endMarker;
        }

        // Destroy the web and its nodes
        DestroyWeb(oldestWeb, startNode, endNode);
    }

    private void UpdateCurrentWeb()
    {
        Vector3 currentEndPoint = CursorManager.Instance.GetCursorWorldPosition();
        currentEndPoint.z = 0;

        if (Vector3.Distance(startPoint, currentEndPoint) > maxWebLength)
        {
            Vector3 direction = (currentEndPoint - startPoint).normalized;
            currentEndPoint = startPoint + (direction * maxWebLength);
        }

        finalEndPoint = currentEndPoint;
        UpdateWebPosition(startPoint, finalEndPoint);
    }

    private void CreateWebObjects()
    {
        currentWeb = Instantiate(webPrefab, startPoint, Quaternion.identity);
        currentWeb.transform.localScale = new Vector3(0.01f, webThickness, 1);

        webCollider = currentWeb.GetComponent<Collider2D>();
        if (webCollider != null) webCollider.enabled = false;

        startMarker = Instantiate(startMarkerPrefab, startPoint, Quaternion.identity);
        activeNodes.Add(startMarker);
        endMarker = Instantiate(endMarkerPrefab, startPoint, Quaternion.identity);
        activeNodes.Add(endMarker);
    }

    private void SetupWebCollision()
    {
        WebCollision webScript = currentWeb.GetComponent<WebCollision>();
        if (webScript != null)
        {
            webScript.startMarker = startMarker;
            webScript.endMarker = endMarker;
        }
    }

    public void ReleaseWeb()
    {
        if (!isDrawing || currentWeb == null) return;

        FinalizeWeb();
        CleanUpCurrentWeb();
        ScheduleWebRemoval();
    }

    private void FinalizeWeb()
    {
        if (webCollider != null) webCollider.enabled = true;

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
    }

    private void CleanUpCurrentWeb()
    {
        isDrawing = false;
        currentWeb = null;
        startMarker = null;
        endMarker = null;
    }

    private void ScheduleWebRemoval()
    {
        if (currentWeb != null && startMarker != null && endMarker != null)
        {
            RemoveWebAfterDelay(currentWeb, startMarker, endMarker, 2f);
        }
    }

    public void DestroyWeb(GameObject web, GameObject startNode, GameObject endNode)
    {
        if (web == null) return;

        activeWebs.Remove(web);
        activeNodes.Remove(startNode);
        activeNodes.Remove(endNode);


        Destroy(web);
        if (startNode != null) Destroy(startNode);
        if (endNode != null) Destroy(endNode);
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
        currentWeb.transform.localScale = new Vector3(length, webThickness, 1);

        UpdateMarkers(start, end);
    }

    private void UpdateMarkers(Vector3 start, Vector3 end)
    {
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
        Vector3 direction = (finalEndPoint - startPoint).normalized;
        return startPoint + direction * currentWeb.transform.localScale.x;
    }

    void RotateMarker(GameObject marker, Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        marker.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
