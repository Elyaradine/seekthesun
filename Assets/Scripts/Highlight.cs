using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField]
    private Material _highlightMaterial;

    [SerializeField]
    private Material _litHighlightMaterial;

    private Material[] _origMaterials;
    private Renderer[] _renderers;

    // Start is called before the first frame update
    void Start()
    {
    }

    void RebuildArrays()
    {
        LineRenderer[] line = GetComponentsInChildren<LineRenderer>();
        if (line.Length > 0)
        {
            _renderers = (Renderer[]) line;
            _origMaterials = new Material[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                _origMaterials[i] = line[i].sharedMaterial;
            }
        }

        if (_origMaterials == null)
        {
            MeshRenderer[] mesh = GetComponentsInChildren<MeshRenderer>();
            if (mesh.Length > 0)
            {
                _renderers = (Renderer[]) mesh;
                _origMaterials = new Material[mesh.Length];
                for (int i = 0; i < mesh.Length; i++)
                {
                    _origMaterials[i] = mesh[i].sharedMaterial;
                }
            }
        }
    }

    void OnDestroy()
    {
        HighlightObject(false);
    }

    public void HighlightObject(bool highlight)
    {
        if (highlight)
        {
            RebuildArrays();
        }

        if (_renderers != null && _renderers.Length > 0)
        {
            for (var i = 0; i < _renderers.Length; i++)
            {
                Renderer r = _renderers[i];

                if (r == null)
                {
                    continue;
                }

                if (highlight)
                {
                    Transform p = r.transform.parent;
                    if (p != null)
                    {
                        Stem s = p.GetComponent<Stem>();
                        if (s != null)
                        {
                            r.sharedMaterial = s.IsLit ? _litHighlightMaterial : _highlightMaterial;
                        }
                    }
                }
                else
                {
                    r.sharedMaterial = _origMaterials[i];
                }
            }
        }
    }
}