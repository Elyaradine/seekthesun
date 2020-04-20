using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float RotateSpeed;
    public float DollySpeed;
    public float AutoRotateSpeed;
    public float MoveSpeed;
    public float MaxHeight;
    public Texture2D WindTexture;
    public Vector2 WindTiling;
    public Vector2 WindSpeed;
    public float WindStrength;

    public float CameraMinDistance, CameraMaxDistance;

    private float _autoRotateSpeed;
    private float _autoRotateDelay;

    private Vector3 _prevMousePosition;

    [SerializeField]
    private Camera _camera;

    void Start()
    {
        Shader.SetGlobalTexture("_WindTex", WindTexture);
        Shader.SetGlobalVector("_WindParams", new Vector4(WindTiling.x, WindTiling.y, WindSpeed.x, WindSpeed.y));
        Shader.SetGlobalFloat("_WindStrength", WindStrength);
        _autoRotateSpeed = 0;

        Vector3 newHeight = transform.position;
        newHeight.y = Mathf.Lerp(0, MaxHeight,
            Mathf.Clamp01((_camera.transform.localPosition.z - CameraMinDistance) /
                          (CameraMaxDistance - CameraMinDistance)));
        transform.position = newHeight;
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_WindParams", new Vector4(WindTiling.x, WindTiling.y, WindSpeed.x, WindSpeed.y));
        Shader.SetGlobalFloat("_WindStrength", WindStrength);

        _autoRotateDelay -= Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up * RotateSpeed * Time.deltaTime);
            ResetAutoRotate();
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.up * RotateSpeed * Time.deltaTime);
            ResetAutoRotate();
        }

        if (Input.GetMouseButtonDown(1))
        {
            _prevMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 diff = Input.mousePosition - _prevMousePosition;
            transform.Rotate(Vector3.up * RotateSpeed * diff.x);

            Vector3 newPos = _camera.transform.localPosition;
            newPos.z += DollySpeed * diff.y;
            newPos.z = Mathf.Clamp(newPos.z, CameraMinDistance, CameraMaxDistance);
            _camera.transform.localPosition = newPos;

            Vector3 newHeight = transform.position;
            newHeight.y = Mathf.Lerp(0, MaxHeight,
                Mathf.Clamp01((newPos.z - CameraMinDistance) / (CameraMaxDistance - CameraMinDistance)));
            transform.position = newHeight;

            _prevMousePosition = Input.mousePosition;

            ResetAutoRotate();
        }

        if (_autoRotateDelay <= 0)
        {
            _autoRotateSpeed += Time.deltaTime * 4;
            _autoRotateSpeed = Mathf.Clamp(_autoRotateSpeed, 0, AutoRotateSpeed);
        }

        transform.Rotate(Vector3.up * _autoRotateSpeed * Time.deltaTime);
    }

    void ResetAutoRotate()
    {
        _autoRotateSpeed = 0;

        _autoRotateDelay = 3.0f;
    }
}