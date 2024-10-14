using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager instance;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFramingTransposer _transposer;

    [Header("Camera distance")]
    private float _targetCameraDistance;
    [Header("是否启用不同武器的相机缩放")]
    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float distanceChangeRate;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (!canChangeCameraDistance) return;

        float currentDistance = _transposer.m_CameraDistance;

        if (Mathf.Abs(_targetCameraDistance - currentDistance) < .01f)
            return;

        _transposer.m_CameraDistance =
            Mathf.Lerp(currentDistance, _targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => _targetCameraDistance = distance;
}