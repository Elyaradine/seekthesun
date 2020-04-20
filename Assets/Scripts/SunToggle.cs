using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SunToggle : MonoBehaviour
{
    [SerializeField]
    private Color _uncheckedColour;

    [SerializeField]
    private Color _checkedColour;

    [SerializeField]
    private float _jiggleStartSize;

    [SerializeField]
    private float _jiggleDuration;

    private Image _image;
    private AudioSource _audioSource;
    private bool _wasEnabled = false;

    void Awake()
    {
        _image = GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
        _wasEnabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetChecked(true);
        }
    }

    public void SetChecked(bool isChecked)
    {
        _image.color = isChecked ? _checkedColour : _uncheckedColour;

        if (isChecked)
        {
            if (!_wasEnabled)
            {
                if (_audioSource != null)
                {
                    _audioSource.Play();
                }
                _wasEnabled = true;
                StartCoroutine(JiggleRoutine());
            }
        }
        else
        {
            _wasEnabled = false;
        }
    }

    IEnumerator JiggleRoutine()
    {
        float ticker = 0;
        while (ticker < _jiggleDuration)
        {
            ticker += Time.deltaTime;

            float progress = Mathf.Clamp01(ticker / _jiggleDuration);
            float jiggle = Utils.ElasticOut(progress);

            transform.localScale = Vector3.one * Mathf.Lerp(_jiggleStartSize, 1.0f, jiggle);
            yield return null;
        }
    }
}