using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PreRenderCamera : MonoBehaviour {

    Camera m_camera;
   // public RenderPlane  renderPlane;
    public Object   planePrefab;


    void Awake()
    {
        m_camera = GetComponent<Camera>();
    }
	// Use this for initialization
	void Start () {

        int divLv = 1;
        CreateRenderPlane(1, Vector3.zero, Vector3.one, Vector3.zero);
        m_camera.orthographicSize = m_camera.orthographicSize * Mathf.Pow(2,divLv);
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyUp(KeyCode.P))
        {
            renderTexture();
        }

	}

    public void CreateRenderPlane(int divideLv, Vector3 pos, Vector3 scale, Vector3 rot)
    {
        if(divideLv == 0)
        {
            GameObject go = GameObject.Instantiate(planePrefab) as GameObject;
            go.transform.parent = transform;
            go.transform.localPosition = pos;
            go.transform.localEulerAngles = rot;
            go.transform.localScale = scale;
            
        }
        else
        {
            for(int i=0; i<4; i++)
            {
                CreateRenderPlane(divideLv - 1, new Vector3(pos.x - Mathf.Pow(-1.0f, i) * 5 * divideLv, pos.y + (i > 1 ? (-1) : 1) * 5 * divideLv, 5), Vector3.one, new Vector3(-90, 0, 0));
            }
        }
        
    }

    IEnumerator SaveTexture()
    {
        yield return new WaitForEndOfFrame();
        RenderTexture rt = new RenderTexture(256, 256, 24, RenderTextureFormat.ARGB32);
        m_camera.targetTexture = rt;
        m_camera.Render();
        RenderTexture oldRt = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        byte[] data = tex.EncodeToPNG();
        FileStream fs = File.Open(Directory.GetCurrentDirectory() + "/Assets/textures/tmp/savePng.png", FileMode.Create);
        fs.Write(data, 0, data.Length);
        fs.Close();
        GameObject.Destroy(tex);
        RenderTexture.active = oldRt;

    }

    public void renderTexture()
    {
        m_camera.Render();
        StartCoroutine(SaveTexture());
    }
}
