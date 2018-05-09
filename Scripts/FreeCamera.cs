using UnityEngine;
using System.Collections;

public class FreeCamera : MonoBehaviour
{
    [Range(0,1000)]
    public float m_XRotSpeed;
    [Range(0, 1000)]
    public float m_YRotSpeed;
    public float m_MoveSpeed;
    public float m_MinVerticalAngle;
    public float m_MaxVerticalAngle;


    private float m_x;
    private float m_y;


    // Use this for initialization
    void Start()
    {
        m_x = transform.rotation.eulerAngles.y;
        m_y = transform.rotation.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire2"))
        {
            m_x += Input.GetAxis("Mouse X") * m_XRotSpeed * 0.02f;
            m_y -= Input.GetAxis("Mouse Y") * m_YRotSpeed * 0.02f;
            m_y = ClampAngle(m_y, m_MinVerticalAngle, m_MaxVerticalAngle);
            Quaternion rot = Quaternion.Euler(m_y, m_x, 0);
            transform.rotation = rot;
        }
       

        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + transform.forward * m_MoveSpeed * 0.01f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position - transform.forward * m_MoveSpeed * 0.01f;
        }

       

        if (transform.rotation.eulerAngles.y < -90 || transform.rotation.eulerAngles.y > 90)
        {
            //Debug.Log(string.Format("m_y:{0}", m_y));
        }


    }
        

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

}
