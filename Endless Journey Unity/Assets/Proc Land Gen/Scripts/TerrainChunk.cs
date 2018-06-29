using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.CFGParser;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class TerrainChunk {
	
	const float colliderGenerationDistanceThreshold = 5;
    const float colliderGenerationDistanceThresholdSqr = colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold;
    const int maxChangesPerFrame = 3;
    const int maxNavMeshSurfaceCount = 3;

    private static int navMeshSurfacesCounter = 0;
    private static int currentChangesPerFrame = 0;

    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
	public Vector2 coord;
	public Vector2 sampleCentre { get; private set; }
    public MeshFilter meshFilter { get; private set; }
    //public NavMeshSurface navMeshSurface;

    private HashSet<ItemComponent> chunkItems;
    private KdTree CurrentMeshKdTree;
    private System.Random rand;

    public static void ResetCurrChangesPerFrameCount() { currentChangesPerFrame = 0; }

    public void AddItem(ItemComponent item)
    {
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

    ItemHandler itemHandler;

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

        chunkItems = new HashSet<ItemComponent>();

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
		Vector2 position = coord * meshSettings.meshWorldSize;
		bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        rand = new System.Random();

        meshObject = new GameObject("Terrain Chunk");
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();
        itemHandler = meshObject.AddComponent<ItemHandler>();
        //navMeshSurface = meshObject.AddComponent<NavMeshSurface>();
        //navMeshSurface.enabled = false; // Default is false
        //navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
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

		UpdateTerrainChunk(calcViewerPosition);
	}

    Vector3 calcViewerPosition
    {
        get
        {
            return new Vector3(viewer.position.x, viewer.position.z, 0);
        }
    }


    public void UpdateTerrainChunk(Vector3? viewerPosition) {
		if (heightMapReceived) {
            var viewerPos = viewerPosition ?? calcViewerPosition;
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));

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
                }

                // Position and enable items once mesh is ready
                if (lodMeshes[lodIndex] != null && lodMeshes[lodIndex].mesh != null && lodMeshes[lodIndex].mesh.vertexCount > 0)
                {
                    itemHandler.positionItemsIter = PositionItems(lodIndex);
                }

                // If CURRENT CHUNK PLAYER IS ON
                // Build navmesh
                //if (viewerDstFromNearestEdge < bounds.extents.x)
                //{
                //    if (!navMeshSurface.enabled && navMeshSurfacesCounter < maxNavMeshSurfaceCount)
                //    {
                //        Debug.Log("Building new navmesh");
                //        navMeshSurface.enabled = true;

                //        // Build nav mesh for agents to travel on
                //        //navMeshSurface.BuildNavMesh();
                //        navMeshSurfacesCounter++;
                //    }
                //}
                //else if (navMeshSurface.enabled)
                //{
                //    Debug.Log("Destroying old navmesh");

                //    // Reset navmesh
                //    navMeshSurface.enabled = false;
                //    navMeshSurfacesCounter--;
                //}
            }
            else
            {
                itemHandler.disableItemsIter = DetachItems();
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
    private IEnumerator<WaitForEndOfFrame> DetachItems()
    {
        // Store any item that was handled by this enumerator
        var handledItems = new HashSet<ItemComponent>();
        var difference = chunkItems.Except(handledItems);

        // Get the first item of the difference
        var item = difference.FirstOrDefault();

        // Select a single item each iteration and remove it.
        while (item != null)
        {
            // Wait this frame
            while (currentChangesPerFrame >= maxChangesPerFrame)
            {
                yield return Globals.EndOfFrame;
            }

            // Disable items to allow them to be returned to the pool and be reused 
            item.GetComponent<ItemComponent>().PreDisable();
            item.gameObject.SetActive(false);

            // Count change
            currentChangesPerFrame++;

            // Wait a frame for next item
            yield return Globals.EndOfFrame;

            // Add item to handled, calc difference again (note that chunkItems may have changed)
            handledItems.Add(item);
            difference = chunkItems.Except(handledItems);

            // Get next item in difference
            item = difference.FirstOrDefault();
        }

        chunkItems.Clear();
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

            groundItem.UpdatePosition(new Vector3(nearestVertex.vertex.x,
                                                    nearestVertex.vertex.y,
                                                    nearestVertex.vertex.z));
        }
    }

    private IEnumerator<WaitForEndOfFrame> PositionItems(int levelOfDetail)
    {
        var sharedMesh = meshFilter.sharedMesh;

        // Verify before placing
        if (sharedMesh != null && sharedMesh.vertexCount > 0)
        {
            // Store any item that was handled by this enumerator
            var handledItems = new HashSet<ItemComponent>();
            var difference = chunkItems.Except(handledItems);

            // Wait this frame
            while (currentChangesPerFrame >= maxChangesPerFrame)
            {
                yield return Globals.EndOfFrame;
            }

            // Get the first item of the difference
            var item = difference.FirstOrDefault();

            // Count item selection
            currentChangesPerFrame++;


            // Select a single item each iteration and remove it.
            while (item != null)
            {
                // GROUNDED ITEMS
                var groundItem = item.GetComponent<GroundItemComponent>();

                // Set our items' Y position and display them
                if (groundItem != null)
                {
                    if (!groundItem.hasPerfectPos)
                    {
                        // Set to true if cacluating for smalled LOD
                        groundItem.hasPerfectPos = levelOfDetail == 0;

                        // Wait this frame
                        while (currentChangesPerFrame >= maxChangesPerFrame)
                        {
                            yield return Globals.EndOfFrame;
                        }

                        // Find nearest vertex where height is half of max possible
                        var nearestVertex = Helpers.NearestVertexTo(meshFilter, CurrentMeshKdTree,
                            new Vector3(groundItem.ActualOriginalPos.x, heightMap.maxValue / 2, groundItem.ActualOriginalPos.y));

                        item.UpdatePosition(new Vector3(nearestVertex.vertex.x,
                                                        nearestVertex.vertex.y,
                                                        nearestVertex.vertex.z));

                        // Count change
                        currentChangesPerFrame++;
                    }

                    // Pause after every positioning. Return false as long as not done
                    yield return Globals.EndOfFrame;
                }

                // Add item to handled, calc difference again (note that chunkItems may have changed)
                handledItems.Add(item);
                difference = chunkItems.Except(handledItems);

                // Get next item in difference
                item = difference.FirstOrDefault();
            }
        }

        // Return null to signify end of iteration
        yield return null;
    }

	public void UpdateCollisionMesh(Vector3? viewerPosition) {
        if (!hasSetCollider) {
            var viewerPos = viewerPosition ?? calcViewerPosition;
			float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPos);

			if (sqrDstFromViewerToEdge < detailLevels [colliderLODIndex].sqrVisibleDstThreshold) {
				if (!lodMeshes[colliderLODIndex].hasRequestedMesh) {
					lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
				}
			}

			if (sqrDstFromViewerToEdge < colliderGenerationDistanceThresholdSqr) {
				if (lodMeshes[colliderLODIndex].hasMesh) {
					meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
					hasSetCollider = true;
				}
			}
		}
    }

    // TODO This is not enough! If walking a large distance game will eventually crash due to 
    // memory allocation. Must destroy chunks after a certain distance
	public void SetVisible(bool visible) {
		meshObject.SetActive(visible);
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
	public event Action<Vector3?> updateCallback;
    public KdTree KdTreeVertices { get; private set; }

    public LODMesh(int lod) {
		this.lod = lod;
	}

	void OnMeshDataReceived(object meshDataObject) {
        var meshData = (MeshData)meshDataObject;
        mesh = meshData.CreateMesh();

        // Get tree from mesh data
        KdTreeVertices = meshData.GetKdTree();
        hasMesh = true;
        
        updateCallback(null);
	}

	public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
		hasRequestedMesh = true;
		ThreadedDataRequester.RequestData (() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
	}

}