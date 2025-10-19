using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 3.5f;
    private Rigidbody rb;
    public float rotateSpeed = 100.0f;
    private Animator anim;
    private int jumpForce =5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Vector3 rotateY = new Vector3(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0);
        if (movement != Vector3.zero)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotateY));
        }
        rb.MovePosition(rb.position + transform.forward * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime+ transform.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime);
        anim.SetFloat("BlendV", Input.GetAxis("Vertical"));
        anim.SetFloat("BlendH", Input.GetAxis("Horizontal"));
    }
    private void Update()

    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}
