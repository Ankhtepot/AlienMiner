using System.Collections.Generic;
using System.Linq;
using Components;
using Utilities.Extensions;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

//Fireball Games * * * PetrZavodny.com

namespace Controllers
{
    public class TerrainSpawner : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private int heightMultiplier = 10;
        [Range(0f, 1f)] [SerializeField] private float sampleScale = 0.1f;
        public Vector2Int InitialWorldOffset;
        public Vector2Int TraversedOffset = new Vector2Int(0, 0);
        [Header("Assignable")]
        [SerializeField] private TerrainElement element;
        [SerializeField] private Transform spawnedElementsParent;
        [SerializeField] private EnvironmentController environment;
        [SerializeField] private PositionMonitor characterPosition;
        [Header("Events")]
        [SerializeField] public CustomUnityEvents.EventVector3Int OnCoordinateHidden;
        [SerializeField] public CustomUnityEvents.EventVector3Int OnCoordinateShown;
        [SerializeField] public CustomUnityEvents.EventBool SpawningFinished;

        private int gridSize;
        private int worldTileSideSize;
        private int XOffset;
        private int YOffset;
        private int ratio;
        private float[,] perlinMap;
        public Dictionary<Vector2Int, TerrainElement> GridMap = new Dictionary<Vector2Int, TerrainElement>();
#pragma warning restore 649

        private readonly Vector2Int ForwardRight = new Vector2Int(1, 1);
        private readonly Vector2Int ForwardLeft = new Vector2Int(-1, 1);
        private readonly Vector2Int BackRight = new Vector2Int(1, -1);
        private readonly Vector2Int BackLeft = new Vector2Int(-1, -1);

        private void Start()
        {
            initialize();
            SpawnNewWorld();
        }

        public List<Vector3Int> GetGroundElementsPositions()
        {
            return GridMap.Select(item => item.Value.transform.position.ToVector3Int()).ToList();
        }

        public bool IsGroundAtPosition(Vector3 position)
        {
            var position2D = position.ToVector2IntXZ();
            return GridMap.ContainsKey(position2D) && GridMap[position2D].transform.position == position;
        }

        private void SetPerlinMap()
        {
            perlinMap = new float[ratio, ratio];

            for (int y = 0; y < ratio; y++)
            {
                for (int x = 0; x < ratio; x++)
                {
                    perlinMap[x, y] = GetPerlinNoiseValue(x + TraversedOffset.x, y + TraversedOffset.y);
                }
            }
        }

        private float GetPerlinNoiseValue(float relativeX, float relativeY)
        {
            var xCoordinate = (InitialWorldOffset.x + relativeX) / gridSize * sampleScale;
            var yCoordinate = (InitialWorldOffset.y + relativeY) / gridSize * sampleScale;

            return Mathf.PerlinNoise(xCoordinate, yCoordinate);
        }

        private void SpawnTerrainElements(bool isNewGame)
        {
            RefreshWorldVariables();
            SetPerlinMap();

            var tilePosition = TraversedOffset.ToVector3();

            for (int x = 0; x < ratio; x++)
            {
                for (int y = 0; y < ratio; y++)
                {
                    var newElement = Instantiate(element, new Vector3(
                            (tilePosition.x + x) * gridSize,
                            Mathf.RoundToInt(perlinMap[x, y] * heightMultiplier),
                            (tilePosition.z + y) * gridSize)
                        , Quaternion.identity);

                    var newElementTransform = newElement.transform;
                    newElementTransform.parent = spawnedElementsParent.transform;

                    GridMap.Add(newElementTransform.position.ToVector2IntXZ(), newElement);
                }
            }

            SpawningFinished?.Invoke(isNewGame);
        }

        private void SpawnNewLine(Vector3Int newPosition)
        {
            if (newPosition.y == 1 || newPosition.y == -1)
            {
                return;
            }

            var moveVector3 = newPosition - characterPosition.OldGridPosition;
            var offsetsChange = moveVector3.ToVector2Int();
            var oldWorldOffset = TraversedOffset;


            if (offsetsChange == Vector2Int.up)
            {
                TraversedOffset += offsetsChange;
                MoveForward(oldWorldOffset);
            }
            else
            if (offsetsChange == Vector2Int.down)
            {
                TraversedOffset += offsetsChange;
                MoveBack(oldWorldOffset);
            }
            else
            if (offsetsChange == Vector2Int.right)
            {
                TraversedOffset += offsetsChange;
                MoveRight(oldWorldOffset);
            }
            else
            if (offsetsChange == Vector2Int.left)
            {
                TraversedOffset += offsetsChange;
                MoveLeft(oldWorldOffset);
            }
            else if (offsetsChange == ForwardRight)
            {
                TraversedOffset += Vector2Int.up;
                MoveForward(oldWorldOffset);
                TraversedOffset += Vector2Int.right;
                MoveRight(oldWorldOffset + Vector2Int.up);
            }
            else if (offsetsChange == ForwardLeft)
            {
                TraversedOffset += Vector2Int.up;
                MoveForward(oldWorldOffset);
                TraversedOffset += Vector2Int.left;
                MoveLeft(oldWorldOffset + Vector2Int.up);
            }
            else if (offsetsChange == BackLeft)
            {
                TraversedOffset += Vector2Int.down;
                MoveBack(oldWorldOffset);
                TraversedOffset += Vector2Int.left;
                MoveLeft(oldWorldOffset + Vector2Int.down);
            }
            else if (offsetsChange == BackRight)
            {
                TraversedOffset += Vector2Int.down;
                MoveBack(oldWorldOffset);
                TraversedOffset += Vector2Int.right;
                MoveRight(oldWorldOffset + Vector2Int.down);
            }
        }

        private void MoveForward(Vector2Int oldWorldOffset)
        {
            for (int i = 0; i < ratio; i++)
            {
                var oldGridPosition = new Vector2Int(i, 0) + oldWorldOffset;
                var newGridPosition = new Vector2Int(i, ratio - 1) + TraversedOffset;

                MoveElement(oldGridPosition, newGridPosition);
            }
        }

        private void MoveLeft(Vector2Int oldWorldOffset)
        {
            for (int i = 0; i < ratio; i++)
            {
                var oldGridPosition = new Vector2Int(ratio - 1, i) + oldWorldOffset;
                var newGridPosition = new Vector2Int(0, i) + TraversedOffset;

                MoveElement(oldGridPosition, newGridPosition);
            }
        }

        private void MoveRight(Vector2Int oldWorldOffset)
        {
            for (int i = 0; i < ratio; i++)
            {
                var oldGridPosition = new Vector2Int(0, i) + oldWorldOffset;
                var newGridPosition = new Vector2Int(ratio - 1, i) + TraversedOffset;

                MoveElement(oldGridPosition, newGridPosition);
            }
        }

        private void MoveBack(Vector2Int oldWorldOffset)
        {
            for (int i = 0; i < ratio; i++)
            {
                var oldGridPosition = new Vector2Int(i, ratio - 1) + oldWorldOffset;
                var newGridPosition = new Vector2Int(i, 0) + TraversedOffset;

                MoveElement(oldGridPosition, newGridPosition);
            }
        }

        private void MoveElement(Vector2Int oldElementKey, Vector2Int newGridPosition)
        {
            float newYHeightValue = GetPerlinNoiseValue(newGridPosition.x, newGridPosition.y);
            var elementToMove = GridMap[oldElementKey];
            var movedTransform = elementToMove.transform;
            var movedPosition = movedTransform.position;

            OnCoordinateHidden?.Invoke(movedPosition.ToVector3Int());

            movedPosition = new Vector3(
                newGridPosition.x,
                Mathf.RoundToInt(newYHeightValue * heightMultiplier),
                newGridPosition.y);
            movedTransform.position = movedPosition;


            GridMap.Remove(oldElementKey);
            GridMap.Add(newGridPosition, elementToMove);

            OnCoordinateShown?.Invoke(movedPosition.ToVector3Int());
        }

        private Vector2Int Generate2DOffset()
        {
            XOffset = Random.Range(0, 1000);
            YOffset = Random.Range(0, 1000);

            return new Vector2Int(XOffset, YOffset);
        }

        //private void InstantiateBuiltElementsFromPosition(List<BuiltElementDescription> dataBuiltElements)
        //{
        //    var buildStore = FindObjectOfType<BuildStoreController>();
        //    var builtElementStore = FindObjectOfType<BuiltElementsStoreController>();

        //    var storeItems = buildStore.GetBuildItemsDictionary();

        //    dataBuiltElements.ForEach(item =>
        //    {
        //        if (!storeItems[item.Name])
        //        {
        //            print($"Item \"{item.Name}\" was not found in buildStore.");
        //            return;
        //        }

        //        var newElement = Instantiate(storeItems[item.Name], item.Position, Quaternion.identity);
        //        newElement.transform.parent = builtElementStore.transform;
        //        newElement.gameObject.SetActive(GridMap.ContainsKey(item.Position.ToVector2Int()));

        //        var health = newElement.GetComponent<BuildElementLifeCycle>();
        //        if (health)
        //        {
        //            health.Hitpoints = item.Health;
        //        }

        //        BuiltElementsStoreController.AddElement(newElement);
        //    });
        //}

        private void ClearElementStores()
        {
            ClearAndDestroyGameObjects(GridMap);

            //ClearAndDestroyGameObjects(BuiltElementsStoreController.GetStoreDictionary());
        }

        //private void SetWorldFromPositionData(SavedPosition data)
        //{
        //    environment.GridSize = data.GridSize;
        //    environment.WorldTileSideSize = data.WorldTileSide;
        //    InitialWorldOffset = data.OriginalOffset;
        //    TraversedOffset = data.TraversedOffset;

        //    FindObjectOfType<CharacterController>().transform.SetPositionAndRotation(
        //        data.CharacterPosition,
        //        data.CharacterRotation);

        //    characterPosition.OldGridPosition = data.CharacterPosition.ToVector3Int();
        //}

        private static void ClearAndDestroyGameObjects<K, V>(IDictionary<K, V> dictionary) where V : MonoBehaviour
        {
            foreach (var item in dictionary)
            {
                Destroy(item.Value.gameObject);
            }

            dictionary.Clear();
        }

        private void RefreshWorldVariables()
        {
            gridSize = environment.GridSize;
            worldTileSideSize = environment.WorldTileSideSize;

            if (worldTileSideSize % gridSize != 0)
            {
                Debug.LogWarning($"World tile side size ({worldTileSideSize}) is not dividable by grid size ({gridSize}) without remainder.");
            }

            ratio = Mathf.RoundToInt(worldTileSideSize / gridSize);
        }

        /// <summary>
        /// Run from SaveLoadController on successful load
        /// </summary>
        /// <param name="data"></param>
        //public void RebuildWorld(SavedPosition data)
        //{
        //    characterPosition.OnPositionChanged.RemoveListener(SpawnNewLine);

        //    var gameController = FindObjectOfType<GameController>();

        //    gameController.InputEnabled = false;

        //    ClearElementStores();

        //    SetWorldFromPositionData(data);

        //    SpawnTerrainElements(false);

        //    InstantiateBuiltElementsFromPosition(data.BuiltElements);

        //    gameController.InputEnabled = true;
        //    characterPosition.OnPositionChanged.AddListener(SpawnNewLine);
        //}

        /// <summary>
        /// Run from GameController OnGameInitiated
        /// </summary>
        public void SpawnNewWorld()
        {
            InitialWorldOffset = Generate2DOffset();
            TraversedOffset = Vector2Int.zero;
            ClearElementStores();
            SpawnTerrainElements(true);
        }

        private void initialize()
        {
            characterPosition.OnPositionChanged.AddListener(SpawnNewLine);
        }
    }
}
