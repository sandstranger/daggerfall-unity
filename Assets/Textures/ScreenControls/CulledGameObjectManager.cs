using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;
using System.Linq;
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
        public const float ScaledBlockRangeSquared = ScaledBlockRange * ScaledBlockRange;

        [SerializeField]
        private GameObject culledObjectsParent;

        private Dictionary<int, CulledGameObject> culledObjects = new Dictionary<int, CulledGameObject>();
        private List<int> keysToRemove = new List<int>();
        private int cullIteration = 0;

        private Vector3 lastPlayerPosition = new Vector3(0, 0, 0);
        private bool wasPlayerInside = false;

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
        private void Start()
        {
            lastPlayerPosition = GameManager.Instance.PlayerMotor.transform.position;
        }
        private void Update()
        {
            Vector3 playerPosition = GameManager.Instance.PlayerMotor.transform.position;
            if (GameManager.Instance.IsPlayerInside != wasPlayerInside)
            {
                cullIteration = 0;
                lastPlayerPosition = playerPosition;
                wasPlayerInside = GameManager.Instance.IsPlayerInside;
            }
            // Remove any deleted gameObjects from the culled objects
            RemoveAnyDeletedObjectsFromCulledDictionary();
            if ((playerPosition - lastPlayerPosition).sqrMagnitude > ScaledBlockRangeSquared / 4) // make sure everything is updated instantly upon teleport
            {
                UpdateAllCullableObjects(playerPosition);
                cullIteration = 10; // jump to iteration 10 so it's a couple frames and then it starts over
            }
            else
                UpdateCullableObjectsBasedOnIteration(cullIteration, playerPosition); // otherwise do a new batch of objects every other frame.
            cullIteration++;
            cullIteration %= 12;
            lastPlayerPosition = playerPosition;
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
        private void UpdateAllCullableObjects(Vector3 playerPosition)
        {
            for (int i = 0; i <= 10; i+=2)
                UpdateCullableObjectsBasedOnIteration(i, playerPosition);
        }
        private void UpdateCullableObjectsBasedOnIteration(int cullIteration, Vector3 playerPosition)
        {
            // Cull and un-cull objects based on range. All objects outside of the ScaledBlockRange distance from player should be culled.
            switch (cullIteration)
            {
                case 0:
                    if (!GameManager.Instance.IsPlayerInside)
                        CullAndUncullDistantObjects(playerPosition, ActiveGameObjectDatabase.GetActiveBillboardObjects(true), 150 * 150);
                    CullAndUncullDistantObjects(playerPosition, ActiveGameObjectDatabase.GetActiveFoeSpawnerObjects(true));
                    break;
                case 2:
                    CullAndUncullDistantObjects(playerPosition, ActiveGameObjectDatabase.GetActiveEnemyObjects(true));
                    CullAndUncullDistantDungeonBlocks(playerPosition, ActiveGameObjectDatabase.GetActiveRDBObjects(true).Where(p => !p.transform.root || p.transform.root.gameObject.name != "Automap"));
                    break;
                case 4:
                    CullAndUncullDistantObjects(playerPosition, ActiveGameObjectDatabase.GetActiveActionDoorObjects(true));
                    break;
                case 6:
                    CullAndUncullDistantObjects(playerPosition, ActiveGameObjectDatabase.GetActiveStaticNPCObjects(true));
                    break;
                case 8:
                    CullAndUncullDistantObjects(playerPosition, ActiveGameObjectDatabase.GetActiveLootObjects(true));
                    break;
                case 10:
                    break;
                //disabled since civilian mobile objects already cull themselves.
                //case 5:
                //    //allCullableObjects.AddRange(ActiveGameObjectDatabase.GetActiveCivilianMobileObjects(true)); 
                //    break;

            }
        }

        private void RemoveAnyDeletedObjectsFromCulledDictionary()
        {
            foreach (var kvp in culledObjects)
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
            keysToRemove.Clear();
        }
        private void CullAndUncullDistantObjects(Vector3 playerPosition, IEnumerable<GameObject> cullableObjects, float maxSquaredDistance = ScaledBlockRangeSquared)
        {
            foreach (GameObject obj in cullableObjects)
            {
                float sqrDistance = (playerPosition - obj.transform.position).sqrMagnitude;
                if (sqrDistance > maxSquaredDistance)
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
        }
        private void CullAndUncullDistantDungeonBlocks(Vector3 playerPosition, IEnumerable<GameObject> cullableRDBObjects)
        {
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
}