using Controllers;
using UnityEngine;
using Utilities;

//Fireball Games * * * PetrZavodny.com

namespace Components
{
    public class PositionMonitor : MonoBehaviour
    {
#pragma warning disable 649
        public Vector3Int CurrentGridPosition;
        public Vector3Int OldGridPosition;
        [SerializeField] public CustomUnityEvents.EventVector3Int OnPositionChanged;
        [SerializeField] private EnvironmentController environment;
#pragma warning restore 649

        private void Start()
        {
            CurrentGridPosition = GetGridPosition();
            OldGridPosition = CurrentGridPosition;
        }

        private void Update()
        {
            ManagePositionChange();
        }

        private void ManagePositionChange()
        {
            var newPosition = GetGridPosition();

            if (newPosition != CurrentGridPosition)
            {
                OldGridPosition = CurrentGridPosition;
                CurrentGridPosition = newPosition;
                OnPositionChanged?.Invoke(newPosition);
            }
        }

        private Vector3Int GetGridPosition()
        {
            return GetGridPosition(transform.position);
        }

        private int NormalizePositionVector(float vector)
        {
            var gridSize = environment.GridSize;
            return Mathf.RoundToInt(vector) * gridSize;
        }

        public Vector3Int GetGridPosition(Vector3 worldPosition)
        {
            return new Vector3Int(
                NormalizePositionVector(worldPosition.x),
                NormalizePositionVector(worldPosition.y),
                NormalizePositionVector(worldPosition.z));
        }
    }
}
