using System;
using System.Collections.Generic;
using Assets.Scripts.CFGParser;
using UnityEngine;
using UnityEngine.AI;

public class TerrainChunk {
	
	const float colliderGenerationDistanceThreshold = 5;
	public event System.Action<TerrainChunk, bool> onVisibilityChanged;
	public Vector2 coord;
	public Vector2 sampleCentre { get; private set; }
    public MeshFilter meshFilter { get; private set; }
    public NavMeshSurface navMeshSurface;

    private List<ItemComponent> chunkItems;
    private KdTree CurrentMeshKdTree;

    public void AddItem(ItemComponent item)
    {
        //item.parent = meshFilter.gameObject.transform;
        item.ParentChunk = this;
        chunkItems.Add(item);
    }

    public void RemoveItem(ItemComponent item)
    {
        chunkItems.Remove(item);
    }

    HeightMap heightMap;
    GameObject meshObject;
	Bounds bounds;

	MeshRenderer meshRenderer;
	
	MeshCollider meshCollider;

	LODInfo[] detailLevels;
	LODMesh[] lodMeshes;
	int colliderLODIndex;

	
	bool heightMapReceived;
	int previousLODIndex = -1;
	bool hasSetCollider;
	float maxViewDst;

	HeightMapSettings heightMapSettings;
	MeshSettings meshSettings;
	Transform viewer;

	public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material) {
		this.coord = coord;
		this.detailLevels = detailLevels;
		this.colliderLODIndex = colliderLODIndex;
		this.heightMapSettings = heightMapSettings;
		this.meshSettings = meshSettings;
		this.viewer = viewer;

        chunkItems = new List<ItemComponent>();

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
		Vector2 position = coord * meshSettings.meshWorldSize;
		bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);


		meshObject = new GameObject("Terrain Chunk");
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();
        navMeshSurface = meshObject.AddComponent<NavMeshSurface>();
		meshRenderer.material = material;

		meshObject.transform.position = new Vector3(position.x,0,position.y);
		meshObject.transform.parent = parent;
		SetVisible(false);

		lodMeshes = new LODMesh[detailLevels.Length];
		for (int i = 0; i < detailLevels.Length; i++) {
			lodMeshes[i] = new LODMesh(detailLevels[i].lod);
			lodMeshes[i].updateCallback += UpdateTerrainChunk;
			if (i == colliderLODIndex) {
				lodMeshes[i].updateCallback += UpdateCollisionMesh;
			}
		}

		maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;

	}

	public void Load() {
		ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
	}



	void OnHeightMapReceived(object heightMapObject) {
		this.heightMap = (HeightMap)heightMapObject;
		heightMapReceived = true;

		UpdateTerrainChunk ();
	}

	Vector2 viewerPosition {
		get {
			return new Vector2 (viewer.position.x, viewer.position.z);
		}
	}


	public void UpdateTerrainChunk() {
		if (heightMapReceived) {
			float viewerDstFromNearestEdge = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));

			bool wasVisible = IsVisible ();
			bool visible = viewerDstFromNearestEdge <= maxViewDst;

            if (visible)
            {
                int lodIndex = 0;

                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                        CurrentMeshKdTree = lodMesh.KdTreeVertices;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(heightMap, meshSettings);
                    }

                    // Position and enable items only when LOD is fine (1 or 0)
                    if (lodIndex <= 1 && lodMeshes[lodIndex] != null && lodMeshes[lodIndex].mesh != null && lodMeshes[lodIndex].mesh.vertexCount > 0)
                    {
                        PositionItems(lodIndex);

                        // Build navmesh only for LOD 0
                        if (lodIndex == 0)
                        {
                            Debug.Log("Building new navmesh");

                            // Build nav mesh for agents to travel on
                            // TODO THIS GETS THE GAME STUCK BECAUSE ITS TOO HEAVY TO COMPUTE?!
                            //navMeshSurface.BuildNavMesh();
                        }
                    }
                }
                
            }
            else
            {
                DetachItems();
            }

            if (wasVisible != visible) {
				
				SetVisible (visible);
				if (onVisibilityChanged != null) {
					onVisibilityChanged (this, visible);
				}
			}
		}
	}

    // Remove items from this chunk's list as it will no longer control them
    private void DetachItems()
    {
        // Lost visibility, remove items
        // Detach!
        foreach (var item in chunkItems)
        {
            // Disable items to allow them to be returned to the pool and be reused 
            item.GetComponent<ItemComponent>().PreDisable();
            item.gameObject.SetActive(false);
        }

        chunkItems.Clear();

        // GROUNDED ITEMS
        //foreach (var item in meshFilter.GetComponentsInChildren<GroundItemComponent>())
        //{
        //    // Destroy the actual gameobject! Not just the script!
        //    //GameObject.Destroy(item.gameObject);
        //}

        // AIRBORNE ITEMS
        //foreach (var item in meshFilter.GetComponentsInChildren<AirborneItemComponent>())
        //{
        //    // Destroy the actual gameobject! Not just the script!
        //    //GameObject.Destroy(item.gameObject);

        //    // Detach!
        //    item.transform.parent = null;
        //}
    }

    // For initiating positions
    public void PositionSingleGroundItem(GroundItemComponent groundItem)
    {
        // Verify before placing
        if (meshFilter.sharedMesh != null && meshFilter.sharedMesh.vertexCount > 0 && previousLODIndex < 2 && !groundItem.hasPerfectPos)
        {
            // Set to true if cacluating for smalled LOD
            groundItem.hasPerfectPos = previousLODIndex == 0;

            // Find nearest vertex where height is half of max possible
            var nearestVertex = Helpers.NearestVertexTo(meshFilter, CurrentMeshKdTree,
                new Vector3(groundItem.ActualOriginalPos.x, heightMap.maxValue / 2, groundItem.ActualOriginalPos.y));

            groundItem.transform.position = new Vector3(nearestVertex.vertex.x,
                                                        nearestVertex.vertex.y,
                                                        nearestVertex.vertex.z);
        }
    }

    private void PositionItems(int levelOfDetail)
    {
        // Verify before placing
        if (meshFilter.sharedMesh != null && meshFilter.sharedMesh.vertexCount > 0)
        {
            foreach (var item in chunkItems)
            {
                // GROUNDED ITEMS
                // Set our items' Y position and display them
                //foreach (var item in meshFilter.GetComponentsInChildren<GroundItemComponent>())
                if (item.GetComponent<GroundItemComponent>() != null)
                {
                    var groundItem = item.GetComponent<GroundItemComponent>();

                    // Enable the item. Shit.
                    //var renderer = GetItemRenderer(item.transform.gameObject);

                    if (!groundItem.hasPerfectPos)
                    {
                        // Set to true if cacluating for smalled LOD
                        groundItem.hasPerfectPos = levelOfDetail == 0;

                        // Find nearest vertex where height is half of max possible
                        var nearestVertex = Helpers.NearestVertexTo(meshFilter, CurrentMeshKdTree,
                            new Vector3(groundItem.ActualOriginalPos.x, heightMap.maxValue / 2, groundItem.ActualOriginalPos.y));

                        item.transform.position = new Vector3(nearestVertex.vertex.x,
                                                              nearestVertex.vertex.y,
                                                              nearestVertex.vertex.z);
                    }
                }

                // AIRBORNE ITEMS
                //foreach (var item in meshFilter.GetComponentsInChildren<AirborneItemComponent>())
                //else if (item.GetComponent<AirborneItemComponent>() != null)
                //{
                //    // Enable the item. Shit.
                //    GetItemRenderer(item.gameObject).enabled = true;
                //}
            }
        }
    }

    //private Renderer GetItemRenderer(GameObject item)
    //{
    //    Renderer renderer = item.GetComponent<MeshRenderer>();

    //    if (renderer == null)
    //    {
    //        renderer = item.GetComponentInChildren<SkinnedMeshRenderer>();
    //    }

    //    return renderer;
    //}

	public void UpdateCollisionMesh() {
		if (!hasSetCollider) {
			float sqrDstFromViewerToEdge = bounds.SqrDistance (viewerPosition);

			if (sqrDstFromViewerToEdge < detailLevels [colliderLODIndex].sqrVisibleDstThreshold) {
				if (!lodMeshes [colliderLODIndex].hasRequestedMesh) {
					lodMeshes [colliderLODIndex].RequestMesh (heightMap, meshSettings);
				}
			}

			if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
				if (lodMeshes [colliderLODIndex].hasMesh) {
					meshCollider.sharedMesh = lodMeshes [colliderLODIndex].mesh;
					hasSetCollider = true;
				}
			}
		}
    }

    // TODO This is not enough! If walking a large distance game will eventually crash due to 
    // memory allocation. Must destroy chunks after a certain distance
	public void SetVisible(bool visible) {
		meshObject.SetActive (visible);
	}

	public bool IsVisible() {
		return meshObject.activeSelf;
	}

}

class LODMesh {
	public Mesh mesh;
	public bool hasRequestedMesh;
	public bool hasMesh;
	int lod;
	public event System.Action updateCallback;
    public KdTree KdTreeVertices { get; private set; }

    public LODMesh(int lod) {
		this.lod = lod;
	}

	void OnMeshDataReceived(object meshDataObject) {
        mesh = ((MeshData)meshDataObject).CreateMesh();

        KdTreeVertices = new KdTree();
        KdTreeVertices.build(mesh.vertices, mesh.triangles);
		hasMesh = true;
        
        updateCallback ();
	}

	public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
		hasRequestedMesh = true;
		ThreadedDataRequester.RequestData (() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
	}

}