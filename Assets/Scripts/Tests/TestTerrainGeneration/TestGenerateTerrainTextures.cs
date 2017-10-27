using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerateTerrainTextures : MonoBehaviour {

    public Material blitMaterial;

    public RenderTexture texture;
    private RenderTexture buffer;

    public Texture initialTexture;

    

	// Use this for initialization
	void Start () {
        Graphics.Blit(initialTexture, texture);  // initialize sim texture from external texture

        buffer = new RenderTexture(texture.width, texture.height, texture.depth, texture.format);
    }

    public void UpdateTexture() {
        Graphics.Blit(texture, buffer, blitMaterial);  // perform calculations on texture
        Graphics.Blit(buffer, texture); // copy results back into main texture
    }
}
