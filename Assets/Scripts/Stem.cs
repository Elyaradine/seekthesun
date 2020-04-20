using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stem : MonoBehaviour
{
    public GameObject LeafPrefab;
    public GameObject FlowerPrefab;
    public Material HighlightMaterial;
    public Plant PlantParent;

    public Vector3 TipPosition => transform.TransformPoint(_lineRenderer.GetPosition(1));

    private LineRenderer _lineRenderer;

    private float _widthFactor;
    private float _heightFactor;

    private float _restingAngle = 0;
    private float _randomAngle = 0;

    private float _ticker;
    private float _tipTicker;
    private float _growthTime = 1.0f;
    private bool _skipUpdate = false;
    private CapsuleCollider _collider;
    private bool _hasEnabled;
    private Material _origMaterial;

    [SerializeField]
    private float _fallSpeed;

    [SerializeField]
    private float _fallAcceleration;

    [SerializeField]
    private bool _canBePruned = true;

    public int StemGrownCount { get; private set; }
    public int LeafGrownCount { get; private set; }
    public int BranchCount { get; private set; }
    public float OffsetRotation { get; private set; }
    public int WindingCount { get; private set; }

    private Vector3 _fallVelocity;
    private bool _isPruned = false;
    public bool IsLit;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.startWidth = _lineRenderer.endWidth = 0.01f;

        _collider = GetComponent<CapsuleCollider>();
        if (_collider != null)
        {
            _collider.enabled = false;
        }

        _ticker = _tipTicker = StemGrownCount = 0;
        _hasEnabled = false;

        _origMaterial = _lineRenderer.sharedMaterial;
    }

    public void RandomizeAroundAxis()
    {
        OffsetRotation = Random.Range(-180.0f, 180.0f);
    }

    public void SetSizeFactor(float widthFactor, float heightFactor)
    {
        _widthFactor = widthFactor;
        _heightFactor = heightFactor;
    }

    public void SetRestingAngle(float angle)
    {
        _restingAngle = angle;
    }

    public void SetBranchCount(int count)
    {
        BranchCount = count;
    }

    public int GrowChildStem()
    {
        StemGrownCount++;
        WindingCount++;
        return StemGrownCount;
    }

    public void OnChildStemPruned()
    {
        StemGrownCount--;
    }

    public void SetColors(Color root, Color tip)
    {
        _lineRenderer.startColor = root;
        _lineRenderer.endColor = tip;
    }

    public void UpdateState(Plant plant)
    {
        if (_skipUpdate)
        {
            return;
        }

        _ticker += Time.deltaTime;

        float progress = Mathf.Clamp01(_ticker / _growthTime);
        float progressElastic = Utils.ElasticOut(progress);

        _lineRenderer.startWidth =
            Mathf.Lerp(0.01f, 1.0f, progressElastic) * Mathf.Pow(_widthFactor, BranchCount) * 0.2f;
        _lineRenderer.SetPosition(1, Vector3.up * progressElastic * _heightFactor);

        Vector3 euler = transform.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(euler.x, euler.y, Mathf.Lerp(0, -_restingAngle, progressElastic));

        if (_collider != null)
        {
            _collider.radius = Mathf.Lerp(0.01f, 1.0f, progressElastic) * Mathf.Pow(_widthFactor, BranchCount) * 0.1f;
            _collider.height = Mathf.Lerp(0.01f, 1.0f, progressElastic) * (_heightFactor + _collider.radius * 2);
            _collider.center = Mathf.Lerp(0.01f, 1.0f, progressElastic) * _heightFactor * 0.5f * Vector3.up;
        }

        if (!_hasEnabled && progress >= 1.0f)
        {
            plant.StemIsAvailable(this);
            if (_collider != null)
            {
                _collider.enabled = true;
            }

            _hasEnabled = true;
        }

        if (StemGrownCount > 0)
        {
            _tipTicker += Time.deltaTime;
            float tipProgress = Mathf.Clamp01(_tipTicker / _growthTime);
            float tipProgressElastic = Utils.ElasticOut(tipProgress);
            _lineRenderer.endWidth =
                Mathf.Lerp(0.01f, Mathf.Pow(_widthFactor, BranchCount + 1), tipProgressElastic) * 0.2f;

            if (tipProgress >= _growthTime)
            {
                _skipUpdate = true;
            }
        }
    }

    public Leaf GrowLeaf(int maxLeavesAllowed)
    {
        Vector3 leafSpawnVector = TipPosition - transform.position;
        float progress = (LeafGrownCount + 1) / ((float) maxLeavesAllowed + 1);

        Leaf leaf = Instantiate(IsLit && progress > 0.7f ? FlowerPrefab : LeafPrefab,
            transform.position + leafSpawnVector * progress,
            Quaternion.identity,
            transform).GetComponent<Leaf>();
        leaf.transform.localEulerAngles = new Vector3(0, 137.5f * LeafGrownCount, 0);
        leaf.ParentStem = this;

        LeafGrownCount++;

        return leaf;
    }

    public void OnClick()
    {
        if (!_canBePruned)
        {
            return;
        }

        var stems = GetComponentsInChildren<Stem>();
        var leaves = GetComponentsInChildren<Leaf>();
        PlantParent.RemoveStems(stems, leaves);

        foreach (Stem stem in stems)
        {
            Highlight h = stem.GetComponent<Highlight>();
            if (h != null)
            {
                Destroy(h);
            }
        }

        _fallVelocity = new Vector3(0, -_fallSpeed, 0);
        _isPruned = true;

        Transform parent = transform.parent;
        if (parent != null)
        {
            Stem parentStem = parent.GetComponent<Stem>();
            if (parentStem != null)
            {
                parentStem.OnChildStemPruned();
                PlantParent.MakeAvailableForNewStem(parentStem);
            }
        }
    }

    void Update()
    {
        if (!_isPruned)
        {
            return;
        }

        _fallVelocity += new Vector3(0, -_fallAcceleration, 0) * Time.deltaTime;
        transform.position += _fallVelocity * Time.deltaTime;

        if (transform.position.y < -50.0f)
        {
            Destroy(gameObject);
        }
    }
}