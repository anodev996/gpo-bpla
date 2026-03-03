using System.Collections;
using Unity.Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Класс обработчик всего инпута
/// </summary>
public class WorldInput : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private CinemachineCamera[] cameras;
    [SerializeField] private CinemachineCamera currentCamera;

    public void ChangeCamera(CinemachineCamera camera)
    {
        currentCamera.gameObject.SetActive(false);
        currentCamera = camera;
        currentCamera.gameObject.SetActive(true);
    }

    public void OpenFreeCamera()
    {
        for(int i = 0; i < cameras.Length; i++)
        {
            if(cameras[i].TryGetComponent<FreeCamera>(out FreeCamera camera))
            {
                ChangeCamera(cameras[i]);
                return;
            }
        }
    }

    public void Open3thCamera(ControlledBody body)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].TryGetComponent<CinemachineThirdPersonFollow>(out CinemachineThirdPersonFollow camera))
            {
                ChangeCamera(cameras[i]);
                return;
            }
        }
    }

    public void EnterControlledBody(ControlledBody body)
    {
        controlled = body;
        predBody = null;
        Open3thCamera(controlled);
    }

    public Transform cameraTransform
    {
        get
        {
            for(int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i].enabled && cameras[i].isActiveAndEnabled)
                {
                    return cameras[i].transform;
                }
            }
            return null;
        }
    }

    public void Update()
    {
        ///Обязательный инпут
        StandartInput();

        if(controlled != null)
        {
            controlled.direction = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Jump"), Input.GetAxis("Horizontal"));
            controlled.angle = Input.GetAxis("Rotation");
        }
    }

    #region Standart
    public ControlledBody controlled { get; set; } 
    private ControlledBody predBody;
    private float lastClickTime;
    public void StandartInput()
    {
        lastClickTime += Time.unscaledDeltaTime;
        if (Input.GetKeyDown(KeyCode.P))
            Time.timeScale = Time.timeScale == 0? 1 : 0;
        if (controlled == null)
        {
            if (MouseRaycast(out Vector3 origin, out RaycastHit hit))
            {
                bool isBreak = false;
                foreach (var body in ControlledBody.bodies)
                {
                    var inputRadius = Mathf.Min(0.01f * Vector3.Distance(camera.transform.position, body.transform.position), body.inputRadius);
                    if (body.inputRadius >= GeometryUtils.GetDistanceToLine(origin, hit.point, body.transform.position))
                    {
                        if (body != predBody)
                        {
                            predBody?.OnUpAim(this);

                            body.OnDownAim(this);
                            predBody = body;
                        }
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (lastClickTime < 0.3f)
                                body.OnDoubleClick(this);
                            else
                                body.OnClick(this);
                            lastClickTime = 0;
                        }
                        isBreak = true;
                        break;
                    }
                }
                if (predBody != null && isBreak == false)
                {
                    predBody.OnUpAim(this);
                    predBody = null;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log($"{controlled} {predBody}");
            if (controlled == null && predBody != null)
                EnterControlledBody(predBody);
            else if (controlled != null)
                OpenFreeCamera();
        }
    }
    #endregion

    public bool MouseRaycast(out Vector3 origin,out RaycastHit hit, float maxDistance = 1000)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        origin = ray.origin;
        return Physics.Raycast(ray, out hit, maxDistance);
    }

    [System.Serializable]
    public enum InputType
    {
        WASD, Mission
    }
}