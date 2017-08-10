using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProceduralCubemap : MonoBehaviour {

    public Material skyboxMaterial;

    public Gradient skyboxGradient;

	// Use this for initialization
	void Awake () {

        //skyboxMaterial.

        Texture2D skyboxMainTexture = new Texture2D(128, 128, TextureFormat.RGB24, true);
        skyboxMainTexture.filterMode = FilterMode.Bilinear;
        for(int x = 0; x < 128; x++) {
            for(int y = 0; y < 128; y++) {
                skyboxMainTexture.SetPixel(x, y, Color.blue);
            }
        }


        Cubemap cubemap = new Cubemap(128, TextureFormat.RGB24, false);
        // PosY:
        for (int x = 0; x < 128; x++) {
            for (int y = 0; y < 128; y++) {
                //float r = Mathf.Lerp(0f, 1f, (float)x / (float)127);
                //float g = Mathf.Lerp(0f, 1f, (float)y / (float)127);
                Color col = GetGradientColor(2f * ((float)x / (float)127 - 0.5f), 1f, 2f * ((float)y / (float)127 - 0.5f));
                cubemap.SetPixel(CubemapFace.PositiveY, x, y, col);
            }
        }
        // PosX:
        for (int x = 0; x < 128; x++) {
            for (int y = 0; y < 128; y++) {
                //float r = Mathf.Lerp(0f, 1f, (float)x / (float)127);
                //float g = Mathf.Lerp(0f, 1f, (float)y / (float)127);
                Color col = GetGradientColor(1f, 2f * ((float)(127 - y) / (float)127 - 0.5f), 2f * ((float)(127 - x) / (float)127 - 0.5f));
                cubemap.SetPixel(CubemapFace.PositiveX, x, y, col);
            }
        }
        // PosZ:
        for (int x = 0; x < 128; x++) {
            for (int y = 0; y < 128; y++) {
                //float r = Mathf.Lerp(0f, 1f, (float)x / (float)127);
                //float g = Mathf.Lerp(0f, 1f, (float)y / (float)127);
                Color col = GetGradientColor(2f * ((float)x / (float)127 - 0.5f), 2f * ((float)(127 - y) / (float)127 - 0.5f), 1f);
                cubemap.SetPixel(CubemapFace.PositiveZ, x, y, col);
            }
        }
        // NegX:
        for (int x = 0; x < 128; x++) {
            for (int y = 0; y < 128; y++) {
                //float r = Mathf.Lerp(0f, 1f, (float)x / (float)127);
                //float g = Mathf.Lerp(0f, 1f, (float)y / (float)127);
                Color col = GetGradientColor(-1f, 2f * ((float)(127 - y) / (float)127 - 0.5f), 2f * ((float)x / (float)127 - 0.5f));
                cubemap.SetPixel(CubemapFace.NegativeX, x, y, col);
            }
        }
        // NegZ:
        for (int x = 0; x < 128; x++) {
            for (int y = 0; y < 128; y++) {
                //float r = Mathf.Lerp(0f, 1f, (float)x / (float)127);
                //float g = Mathf.Lerp(0f, 1f, (float)y / (float)127);
                Color col = GetGradientColor(2f * ((float)(127 - x) / (float)127 - 0.5f), 2f * ((float)(127 - y) / (float)127 - 0.5f), -1f);
                cubemap.SetPixel(CubemapFace.NegativeZ, x, y, col);
            }
        }
        // NegY:
        for (int x = 0; x < 128; x++) {
            for (int y = 0; y < 128; y++) {
                //float r = Mathf.Lerp(0f, 1f, (float)x / (float)127);
                //float g = Mathf.Lerp(0f, 1f, (float)y / (float)127);
                Color col = GetGradientColor(2f * ((float)x / (float)127 - 0.5f), -1f, 2f * ((float)(127 - y) / (float)127 - 0.5f));
                cubemap.SetPixel(CubemapFace.NegativeY, x, y, col);
            }
        }
        cubemap.Apply();

        skyboxMaterial.mainTexture = skyboxMainTexture;
        skyboxMaterial.SetTexture(Shader.PropertyToID("_Tex"), cubemap);

        DynamicGI.UpdateEnvironment();
    }

    public Color GetGradientColor(float x, float y, float z) {
        //return new Color(x, y, z);
        //return Color.Lerp(Color.black, Color.white, GetLatitude(x, y, z));
        return skyboxGradient.Evaluate(GetLatitude(x, y, z));
    }

    public float GetLatitude(float x, float y, float z) {
        float lat = Vector3.Dot(new Vector3(0f, 1f, 0f), new Vector3(x, y, z).normalized) * 0.5f + 0.5f;
        return lat;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
