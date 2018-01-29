using UnityEngine;
using System.Collections;
using Assets.Scripts.CFGParser;

public class MapPreview : MonoBehaviour {

	public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;


	public enum DrawMode {NoiseMap, Mesh, FalloffMap};
	public DrawMode drawMode;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureData;

	public Material terrainMaterial;

    //public GameObject objectToPlace;
    //public Vector2 actualPos;


	[Range(0,MeshSettings.numSupportedLODs-1)]
	public int editorPreviewLOD;
	public bool autoUpdate;




	public void DrawMapInEditor() {
		textureData.ApplyToMaterial (terrainMaterial);
		textureData.UpdateMeshHeights (terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
		HeightMap heightMap = HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);

		if (drawMode == DrawMode.NoiseMap) {
			DrawTexture (TextureGenerator.TextureFromHeightMap (heightMap));
		} else if (drawMode == DrawMode.Mesh) {
            var data = MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD);

            DrawMesh (data);

            // Sagie: Place an item
            //GenItem(data);

		} else if (drawMode == DrawMode.FalloffMap) {
			DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine),0,1)));
		}
	}

    //private void GenItem(MeshData meshData)
    //{
    //    var chunkMesh = GetComponentInChildren<MeshFilter>();

    //    // Destroy prevs
    //    foreach (var groundItem in chunkMesh.GetComponentsInChildren<GroundItemComponent>())
    //    {
    //        DestroyImmediate(groundItem.gameObject);
    //    }

    //    var item = GameObject.Instantiate(objectToPlace);
    //    item.transform.parent = chunkMesh.transform;

    //    // Place on actual Position
    //    item.transform.position = new Vector3(actualPos.x, 0, actualPos.y);

    //    // Find vertex positioned closest to the bottom of this item on the XZ plane. Get its Y value
    //    var widthPercent = ((item.transform.localPosition.x + 25) / 50);
    //    var heightPercent = 1 - ((item.transform.localPosition.z + 25) / 50);
    //    var indexX = Mathf.RoundToInt(meshData.numMainVerticesPerLine * widthPercent);
    //    var indexZ = Mathf.RoundToInt(meshData.numMainVerticesPerLine * heightPercent);

    //    // The math: n = number of verts per line. 2n-2 vertices are outside the main vertex matrix. 
    //    // Then for i = number of rows we get 4 extra vertices starting at i = 0.
    //    // The the final calculation is (2n-2+(i+1)*4) + (mainVerts * i + j)
    //    var belowItemVertex = chunkMesh.sharedMesh.vertices[
    //        ((2 * meshData.numVertsPerLine) - 2 + ((indexX + 1) * 4)) +
    //        ((meshData.numMainVerticesPerLine * indexX) + indexZ)];

    //    var data = Helpers.NearestVertexTo(meshFilter, item.transform.position);
    //    item.transform.localPosition = new Vector3(data.vertex.x, data.vertex.y, data.vertex.z);

    //    item.GetComponent<Renderer>().enabled = true;
    //}



	public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height) /10f;

		textureRender.gameObject.SetActive (true);
		meshFilter.gameObject.SetActive (false);
	}

	public void DrawMesh(MeshData meshData) {
		meshFilter.sharedMesh = meshData.CreateMesh ();

		textureRender.gameObject.SetActive (false);
		meshFilter.gameObject.SetActive (true);
	}



	void OnValuesUpdated() {
		if (!Application.isPlaying) {
			DrawMapInEditor ();
		}
	}

	void OnTextureValuesUpdated() {
		textureData.ApplyToMaterial (terrainMaterial);
	}

	void OnValidate() {

		if (meshSettings != null) {
			meshSettings.OnValuesUpdated -= OnValuesUpdated;
			meshSettings.OnValuesUpdated += OnValuesUpdated;
		}
		if (heightMapSettings != null) {
			heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
			heightMapSettings.OnValuesUpdated += OnValuesUpdated;
		}
		if (textureData != null) {
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
		}

	}

}
