using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour 
{
    [SerializeField]
    private float walkFowardspeed = 5f;
    [SerializeField]
    private float walkBackSpeed = 2f;

    [SerializeField]
    private float rotateSensitivity = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [Header("Spring settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    // Component cashing
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    void Start()
    {
        motor = this.GetComponent<PlayerMotor>();
        joint = this.GetComponent<ConfigurableJoint>();
        animator = this.GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    void Update()
    {
        if (PauseMenu.IsOn)
        {
            if (Cursor.lockState != CursorLockMode.None)
                Cursor.lockState = CursorLockMode.None;

            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            motor.CameraRotate(0f);

            return;
        }     

        if(Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Calculate movement velocity as a 3D vector
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMov; // (1, 0, 0)
        Vector3 moveVertical = transform.forward * zMov; // (0, 0, 1)

        // Final movement vector
        Vector3 velocity = Vector3.zero;

        if(zMov > -0.1f)
            velocity = (moveHorizontal + moveVertical) * walkFowardspeed;
        else
            velocity = (moveHorizontal + moveVertical) * walkBackSpeed;

        animator.SetFloat("Speed", zMov);
        animator.SetFloat("Direction", xMov);

        //Apply movement
        motor.Move(velocity);

        //Calculate rotation as a 3D vector (Turning around)
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * rotateSensitivity;

        //Apply rotation
        motor.Rotate(rotation);

        //Calculate camera rotation as a 3D vector (Turning around)
        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * rotateSensitivity;

        //Apply camera rotation
        motor.CameraRotate(cameraRotationX);

        Vector3 p_thrusterForce = Vector3.zero;
       
        //Calculate the thruster force based on plyer's input
        if(Input.GetButton("Jump"))
        {
            if (!animator.IsInTransition(0))
            {
                p_thrusterForce = Vector3.up * thrusterForce;
                animator.SetBool("Jump", true);
                SetJointSettings(0f); 
            }                    
        }
        else
        {
            animator.SetBool("Jump", false);
            SetJointSettings(jointSpring);
        }

        //Apply the thruster force
        motor.ApplyThruster(p_thrusterForce);
    }

    private void SetJointSettings(float p_jointSpring)
    {
        joint.yDrive = new JointDrive {
            positionSpring = p_jointSpring, 
            maximumForce = jointMaxForce
        };
    }
}
