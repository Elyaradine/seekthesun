using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public GameObject StemPrefab;
    public SunToggle SunTargetPrefab;
    public Material MoundMaterial;

    public int SplitCount = 2;
    public int LeavesPerStem = 3;
    public float WidthFactor = 0.8f;
    public float HeightFactor = 0.8f;
    public float StemAngle = 30.0f;
    public float TimeToFullGrowth = 1.0f;
    public Color[] StemColourSteps;
    public Color[] LeafColours;

    public int StartingEnergy = 10;

    private List<Stem> _stemList;
    private List<Stem> _availableStems;
    private List<Stem> _leafStems;
    private List<Leaf> _leaves;
    private int _stemCounter;
    private int _furthestBranch = 0;
    private float _newLeafTicker = 0;
    private bool _canGrowStems = false;
    private float _newStemTicker = 0;

    private int _maxEnergy;
    private int _currentEnergy;
    private LightBeam[] _lightBeams;

    [SerializeField]
    private TextMeshProUGUI _energyDisplay;

    [SerializeField]
    private Transform _sunTargetPanel;

    [SerializeField]
    private CanvasGroup _endGameGroup;

    private SunToggle[] _sunToggles;
    private bool _hasEnded = false;

    void Awake()
    {
        _stemList = new List<Stem>();
        _availableStems = new List<Stem>();
        _leafStems = new List<Stem>();
        _leaves = new List<Leaf>();

        Stem startingStem = GetComponentInChildren<Stem>();
        startingStem.SetSizeFactor(WidthFactor, HeightFactor);
        _stemList.Add(startingStem);
        _availableStems.Add(startingStem);

        _newStemTicker = Random.Range(1.0f, 2.0f);
        _newLeafTicker = Random.Range(1.0f, 2.0f);

        _lightBeams = FindObjectsOfType<LightBeam>();
        _sunToggles = new SunToggle[_lightBeams.Length];
        for (int i = 0; i < _lightBeams.Length; i++)
        {
            Debug.Log($"spawn for {i}");
            _sunToggles[i] = Instantiate(SunTargetPrefab, _sunTargetPanel);
        }

        _maxEnergy = _currentEnergy = StartingEnergy;
    }

    void Start()
    {
        UpdateEnergyDisplay();
        UpdateStemColours();
    }

    public void GrowNewStem()
    {
        if (_availableStems.Count < 1)
        {
            return;
        }

        Stem stem = _availableStems[Random.Range(0, _availableStems.Count)];
        Stem newStem = Instantiate(StemPrefab, stem.TipPosition, stem.transform.rotation, stem.transform)
            .GetComponent<Stem>();

        Vector3 stemeuler = new Vector3(0, stem.OffsetRotation + 137.5f * stem.WindingCount, 0);
        newStem.transform.localRotation = Quaternion.Euler(stemeuler);
        newStem.PlantParent = this;
        newStem.RandomizeAroundAxis();
        newStem.SetRestingAngle(StemAngle);
        newStem.SetSizeFactor(WidthFactor, HeightFactor);
        newStem.SetBranchCount(stem.BranchCount + 1);
        _furthestBranch = Mathf.Max(_furthestBranch, stem.BranchCount + 1);
        newStem.gameObject.name = $"Stem_{_stemCounter++}";
        _stemList.Add(newStem);
        _leafStems.Add(newStem);

        if (stem.GrowChildStem() >= SplitCount)
        {
            MakeUnavailable(stem);
        }

        UpdateStemColours();
    }

    public void GrowNewLeaf()
    {
        if (_leafStems.Count < 1)
        {
            return;
        }

        Stem stem = _leafStems[Random.Range(0, _leafStems.Count)];
        Leaf leaf = stem.GrowLeaf(LeavesPerStem);
        _leaves.Add(leaf);
        if (stem.LeafGrownCount >= LeavesPerStem)
        {
            _leafStems.Remove(stem);
        }
    }

    void Update()
    {
        CheckForGrowingStems();
        CheckForGrowingLeaves();

        UpdateStems();
        UpdateLeaves();
        UpdateLightBeamDisplay();
    }

    void CheckForGrowingStems()
    {
        if (!_canGrowStems || _currentEnergy <= 0)
        {
            return;
        }

        _newStemTicker -= Time.deltaTime;
        if (_newStemTicker <= 0)
        {
            GrowNewStem();
            _currentEnergy--;
            UpdateEnergyDisplay();
            _newStemTicker = Random.Range(1.0f, 2.0f);
        }
    }

    void UpdateEnergyDisplay()
    {
        if (_currentEnergy <= 0)
        {
            _energyDisplay.text = $"Energy: <color=#d03434>{_currentEnergy}</color>/{_maxEnergy}";
        }
        else
        {
            _energyDisplay.text = $"Energy: {_currentEnergy}/{_maxEnergy}";
        }
    }

    public void UpdateLightBeamDisplay()
    {
        int count = 0;
        foreach (LightBeam lightBeam in _lightBeams)
        {
            if (lightBeam.BeamHit)
            {
                count++;
            }
        }

        for (int i = 0; i < _sunToggles.Length; i++)
        {
            _sunToggles[i].SetChecked(i < count);
        }

        if (count >= _sunToggles.Length && !_hasEnded)
        {
            _hasEnded = true;
            StartCoroutine(EndGameRoutine());
        }
    }

    IEnumerator EndGameRoutine()
    {
        _endGameGroup.gameObject.SetActive(true);

        float ticker = 0;
        while (ticker <= 1)
        {
            ticker += Time.deltaTime;
            _endGameGroup.alpha = Mathf.Clamp01(ticker) * 0.25f;
            yield return null;
        }

        yield return new WaitForSeconds(10.0f);

        ticker = 1.0f;
        while (ticker > 0)
        {
            ticker -= Time.deltaTime;
            _endGameGroup.alpha = Mathf.Clamp01(ticker) * 0.25f;
            yield return null;
        }

        _endGameGroup.gameObject.SetActive(false);
    }

    void CheckForGrowingLeaves()
    {
        _newLeafTicker -= Time.deltaTime;
        if (_newLeafTicker <= 0)
        {
            GrowNewLeaf();
            _newLeafTicker = Random.Range(0.5f, 2.5f);
        }
    }

    void UpdateStems()
    {
        foreach (Stem stem in _stemList)
        {
            stem.UpdateState(this);
        }
    }

    void UpdateLeaves()
    {
        foreach (Leaf leaf in _leaves)
        {
            leaf.UpdateState();
        }
    }

    public void StemIsAvailable(Stem stem)
    {
        if (Vector3.Dot(stem.transform.up, Vector3.up) >= 0.1f && !_availableStems.Contains(stem))
        {
            _availableStems.Add(stem);
        }
    }

    void UpdateStemColours()
    {
        int offset = Mathf.Clamp(StemColourSteps.Length - _furthestBranch, 0, StemColourSteps.Length);
        foreach (Stem stem in _stemList)
        {
            Color rootColour = StemColourSteps[Mathf.Clamp(stem.BranchCount + offset, 0, StemColourSteps.Length - 1)];
            Color tipColour =
                StemColourSteps[Mathf.Clamp(stem.BranchCount + 1 + offset, 0, StemColourSteps.Length - 1)];
            stem.SetColors(rootColour, tipColour);
        }

        MoundMaterial.SetColor("_Color", StemColourSteps[Mathf.Clamp(offset, 0, StemColourSteps.Length - 1)]);
    }

    void MakeUnavailable(Stem stem)
    {
        if (_availableStems.Contains(stem))
        {
            _availableStems.Remove(stem);
        }
    }

    public void MakeAvailableForNewStem(Stem stem)
    {
        if (!_availableStems.Contains(stem))
        {
            _availableStems.Add(stem);
        }
    }

    public void RemoveStems(Stem[] stems, Leaf[] leaves)
    {
        foreach (Stem stem in stems)
        {
            int availableIndex = _availableStems.IndexOf(stem);
            if (availableIndex > -1)
            {
                _availableStems.RemoveAt(availableIndex);
//                _availableStems.Remove(stem);
            }

            if (_leafStems.Contains(stem))
            {
                _leafStems.Remove(stem);
            }

            if (_stemList.Contains(stem))
            {
                _stemList.Remove(stem);
            }
        }

        foreach (Leaf leaf in leaves)
        {
            if (_leaves.Contains(leaf))
            {
                _leaves.Remove(leaf);
            }
        }

        _currentEnergy += stems.Length;
        UpdateEnergyDisplay();
    }

    public void SetGrowingLoopEnabled(bool enabled)
    {
        _canGrowStems = enabled;
    }

    public void AddMaxEnergy(int energy)
    {
        _maxEnergy += energy;
        _currentEnergy += energy;
        UpdateEnergyDisplay();
    }
}