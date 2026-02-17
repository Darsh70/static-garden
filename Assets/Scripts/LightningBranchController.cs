using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LightningBranchController : MonoBehaviour
{
    private LineRenderer _lr;
    private Camera _camera;

    [Header("Fractal Settings")]
    public int detailLevels = 6;
    public float initialOffset = 2f;
    public float roughness = 0.5f;

    [Header("Appearance")]
    public float strikeWidth = 0.15f;
    public float fadeDuration = 0.2f;

    [Header("The Juice")]
    public GameObject sparkPrefab;      // Particle system for the hit point

    public float shakeIntensity = 0.2f; 
    public GameObject ghostPrefab;     
    public float microJitter = 0.05f;   // Vibration while the bolt exists

    private List<Vector3> _pathPoints = new List<Vector3>();

    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        _camera = Camera.main;
        _lr.enabled = false;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StopAllCoroutines();
            StartCoroutine(FractalStrike());
        }
    }

    IEnumerator FractalStrike()
    {
        _lr.enabled = true;
        
        Vector3 start = transform.position;
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 end = _camera.ScreenToWorldPoint(mousePos);
        end.z = 0f;

        // Generate fractal path
        _pathPoints.Clear();
        _pathPoints.Add(start);
        _pathPoints.Add(end);

        float currentOffset = initialOffset;
        for (int i = 0; i < detailLevels; i++)
        {
            for (int j = 0; j < _pathPoints.Count - 1; j += 2)
            {
                Vector3 mid = GetFractalMidpoint(_pathPoints[j], _pathPoints[j + 1], currentOffset);
                _pathPoints.Insert(j + 1, mid);
            }
            currentOffset *= roughness;
        }

        
        _lr.positionCount = _pathPoints.Count;
        _lr.SetPositions(_pathPoints.ToArray());


        // Impact
        // StartCoroutine(CameraShake(0.1f, shakeIntensity));
        // if (sparkPrefab) Instantiate(sparkPrefab, end, Quaternion.identity);
        SpawnRetinalBurn(_pathPoints); // Creates the lingering after-image

        // Fade 
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / fadeDuration;

            // Fading Width
            _lr.startWidth = Mathf.Lerp(strikeWidth, 0, p);
            _lr.endWidth = _lr.startWidth;

            // Jitter
            // for (int i = 1; i < _pathPoints.Count - 1; i++)
            // {
            //     Vector3 jitter = Random.insideUnitSphere * microJitter;
            //     _lr.SetPosition(i, _pathPoints[i] + jitter);
            // }

            yield return null;
        }

        _lr.enabled = false;
    }

    void SpawnRetinalBurn(List<Vector3> points)
    {
        if (ghostPrefab == null) return;
        GameObject ghost = Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
        LineRenderer ghostLR = ghost.GetComponent<LineRenderer>();
        ghostLR.positionCount = points.Count;
        ghostLR.SetPositions(points.ToArray());
    }

    IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = _camera.transform.localPosition;
        float elapsed = 0;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            _camera.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _camera.transform.localPosition = originalPos;
    }

    Vector3 GetFractalMidpoint(Vector3 a, Vector3 b, float offset)
    {
        Vector3 mid = (a + b) / 2f;
        Vector3 dir = (b - a).normalized;
        Vector3 perpendicular = new(-dir.y, dir.x, 0);
        return mid + (perpendicular * Random.Range(-offset, offset));
    }
}