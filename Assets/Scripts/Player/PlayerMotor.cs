using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour 
{
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rigid;

    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();
    }

    // Gets a movement vector
    public void Move(Vector3 p_velocity)
    {
        velocity = p_velocity;
    }
    
    // Gets a rotational vector
    public void Rotate(Vector3 p_rotation)
    {
        rotation = p_rotation;
    }

    // Gets a rotational vector for the camera
    public void CameraRotate(float p_cameraRotationX)
    {
        cameraRotationX = p_cameraRotationX;
    }

    // Get a vector for thruster
    public void ApplyThruster(Vector3 p_thrusterForce)
    {
        thrusterForce = p_thrusterForce;
    }

    // Run every physics iteration
    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
           rigid.MovePosition(rigid.position + velocity * Time.fixedDeltaTime); // MovePosition은 콜라이더에 부딪치면 멈춤
        }

        if(thrusterForce != Vector3.zero)
        {
            rigid.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation()
    {
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(rotation));
        if(cam != null)
        {
            // Set our rotation and clamp it
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            // Apply our rotation to the transform of our camera
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }
}
