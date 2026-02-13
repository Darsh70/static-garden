using UnityEngine;

public enum CellType { Empty, Plant, Fungus }

public class Cell : MonoBehaviour
{
    public int x, y;
    public CellType type = CellType.Empty;
    
    [Header("Infection Logic")]
    public bool isStruggling = false; 
    private float _infectionTimer = 0f;
    private float _timeToConvert = 2.0f;

    [Header("Visuals")]
    public GameObject plantVisual;
    public GameObject fungusVisual;
    public SpriteRenderer plantRenderer;

    public float immunityTimer = 0f;

    public void SetType(CellType newType)
    {
        type = newType;
        isStruggling = false;
        _infectionTimer = 0;
        UpdateVisuals();
    }

    public void StartInfection()
    {
        // Cannot be infected if immune
        if (type == CellType.Plant && !isStruggling && immunityTimer <= 0)
        {
            isStruggling = true;
            _infectionTimer = _timeToConvert;
        }
    }

    public void Purge()
    {
        if (type == CellType.Fungus)
        {
            SetType(CellType.Empty);
        }
        else if (type == CellType.Plant)
        {
            // cleanse infected plant
            isStruggling = false;
            _infectionTimer = 0;
            immunityTimer = 4.0f; 
            UpdateVisuals();
        }
    }


    void Update()
    {
        if (immunityTimer > 0) immunityTimer -= Time.deltaTime;

        if (isStruggling)
        {
            _infectionTimer -= Time.deltaTime;

            
            float shake = Mathf.Lerp(0.2f, 0.05f, _infectionTimer / _timeToConvert);
            plantVisual.transform.localPosition = Random.insideUnitSphere * shake;
            
            plantRenderer.color = Color.Lerp(new Color(0.83f, 0.23f, 0.41f), Color.white, _infectionTimer / _timeToConvert);

            if (_infectionTimer <= 0)
            {
                SetType(CellType.Fungus);
                plantVisual.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void UpdateVisuals()
    {
        if (plantVisual) plantVisual.SetActive(type == CellType.Plant);
        if (fungusVisual) fungusVisual.SetActive(type == CellType.Fungus);
        if (type == CellType.Plant) plantRenderer.color = Color.white; 
    }
}