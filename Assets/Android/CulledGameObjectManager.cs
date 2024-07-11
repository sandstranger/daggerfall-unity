using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public struct CulledGameObject
    {
        public GameObject gameObject;
        public Transform originalParent;
        public Vector3 originalLocalPosition;
        public Quaternion originalLocalRotation;
        public bool wasOriginalParentNull;
        public bool wasObjectInside;
    }

    public class CulledGameObjectManager : MonoBehaviour
    {
        public static CulledGameObjectManager Instance { get; private set; }
        public const float UnscaledBlockRange = 2060;
        public const float ScaledBlockRange = UnscaledBlockRange * MeshReader.GlobalScale;

        [SerializeField]
        private GameObject culledObjectsParent;

        private Dictionary<int, CulledGameObject> culledObjects = new Dictionary<int, CulledGameObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            if (culledObjectsParent == null)
            {
                culledObjectsParent = new GameObject("CulledObjectsParent");
                culledObjectsParent.SetActive(false);
            }
        }

        private void Update()
        {
            if (Time.frameCount % 66 == 0)
            {
                // Remove any deleted gameObjects from the culled objects
                List<int> keysToRemove = new List<int>();
                foreach(var kvp in culledObjects)
                {
                    if (!kvp.Value.gameObject || !kvp.Value.wasOriginalParentNull && !kvp.Value.originalParent || kvp.Value.wasObjectInside != GameManager.Instance.IsPlayerInside)
                        keysToRemove.Add(kvp.Key);
                }
                foreach (int k in keysToRemove)
                {
                    if (culledObjects[k].gameObject)
                        Destroy(culledObjects[k].gameObject);
                    culledObjects.Remove(k);
                }

                // Get cullable objects, etc
                Vector3 playerPosition = GameManager.Instance.PlayerMotor.transform.position;
                List<GameObject> allCullableObjects = new List<GameObject>();
                allCullableObjects.AddRange(ActiveGameObjectDatabase.GetActiveActionDoorObjects(true));
                //allCullableObjects.AddRange(ActiveGameObjectDatabase.GetActiveCivilianMobileObjects(true)); //disabled since civilian mobile objects already cull themselves.
                allCullableObjects.AddRange(ActiveGameObjectDatabase.GetActiveEnemyObjects(true));
                allCullableObjects.AddRange(ActiveGameObjectDatabase.GetActiveFoeSpawnerObjects(true));
                allCullableObjects.AddRange(ActiveGameObjectDatabase.GetActiveLootObjects(true));
                allCullableObjects.AddRange(ActiveGameObjectDatabase.GetActiveStaticNPCObjects(true));

                // Cull and un-cull objects based on range. All objects outside of the ScaledBlockRange distance from player should be culled.
                foreach (GameObject obj in allCullableObjects)
                {
                    float distance = Vector3.Distance(playerPosition, obj.transform.position);
                    if (distance > ScaledBlockRange)
                    {
                        if (!IsObjectCulled(obj))
                        {
                            CullObject(obj);
                        }
                    }
                    else
                    {
                        if (IsObjectCulled(obj))
                        {
                            UnCullObject(obj);
                        }
                    }
                }

                // Cull and un-cull dungeon blocks based on range.
                List<GameObject> cullableRDBObjects = new List<GameObject>();
                cullableRDBObjects.AddRange(ActiveGameObjectDatabase.GetActiveRDBObjects(true));
                cullableRDBObjects.RemoveAll(p => p.transform.root && p.transform.root.gameObject.name == "Automap");
                foreach (GameObject block in cullableRDBObjects)
                {
                    // Constructing bounds manually based on the block's footprint and pivot information
                    Vector3 blockCenter = block.transform.position + Vector3.one * (1024 * MeshReader.GlobalScale); // Center of the block
                    Vector3 blockSize = Vector3.one * (2048 * MeshReader.GlobalScale); // Assuming infinite height for simplicity
                    Bounds blockBounds = new Bounds(blockCenter, blockSize);

                    // Check if the player is within the ScaledBlockRange from the closest point on the bounds
                    float closestPointDistance = Vector3.Distance(playerPosition, blockBounds.ClosestPoint(playerPosition));
                    if (closestPointDistance > ScaledBlockRange && !blockBounds.Contains(playerPosition)) // Check if outside range and player not inside block
                    {
                        if (!IsObjectCulled(block))
                        {
                            CullObject(block);
                        }
                    }
                    else
                    {
                        if (IsObjectCulled(block))
                        {
                            UnCullObject(block);
                        }
                    }
                }
            }
        }

        public bool CullObject(GameObject objectToCull)
        {
            int objectToCullID = objectToCull.GetInstanceID();
            if (objectToCull == null || culledObjects.ContainsKey(objectToCullID))
                return false;

            CulledGameObject culledObject = new CulledGameObject
            {
                gameObject = objectToCull,
                originalParent = objectToCull.transform.parent,
                originalLocalPosition = objectToCull.transform.localPosition,
                originalLocalRotation = objectToCull.transform.localRotation,
                wasOriginalParentNull = objectToCull.transform.parent == null,
                wasObjectInside = GameManager.Instance.IsPlayerInside
            };

            objectToCull.transform.SetParent(culledObjectsParent.transform, true);
            culledObjects[objectToCullID] = culledObject;
            return true;
        }

        public bool UnCullObject(GameObject objectToUnCull)
        {
            int objectToUncullID = objectToUnCull.GetInstanceID();
            if (!culledObjects.ContainsKey(objectToUncullID))
                return false;
            CulledGameObject culledObject = culledObjects[objectToUncullID];

            if (culledObject.gameObject != null)
            {
                if (!culledObject.wasOriginalParentNull && !culledObject.originalParent) // If the original parent was destroyed
                {
                    Destroy(culledObject.gameObject); // Destroy the object, since it would have been destroyed along with the parent
                }
                else // Parent exists or it didn't have one in the first place. Restore the original parent and transform
                {
                    culledObject.gameObject.transform.SetParent(culledObject.originalParent, true);
                    culledObject.gameObject.transform.SetLocalPositionAndRotation(culledObject.originalLocalPosition, culledObject.originalLocalRotation);
                }
            }
            culledObjects.Remove(objectToUncullID);
            return true;
        }

        public bool IsObjectCulled(GameObject obj)
        {
            return obj && culledObjects.ContainsKey(obj.GetInstanceID());
        }
    }
}