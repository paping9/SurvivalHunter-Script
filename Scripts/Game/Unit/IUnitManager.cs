using Defs;
using UnityEngine;

namespace Game
{
    public interface IUnitManager
    {
        /// <summary>
/// Creates a new unit and registers it with the unit manager.
/// </summary>
void CreateUnit();
        /// <summary>
/// Spawn a unit with the specified identifier at the given world position.
/// </summary>
/// <param name="unitId">Identifier of the unit to spawn.</param>
/// <param name="position">World-space position where the unit will be placed.</param>
void SpawnUnit(int unitId, Vector3 position);
        /// <summary>
/// Removes the unit with the specified identifier from the game and releases any associated resources or state.
/// </summary>
/// <param name="unitId">The identifier of the unit to despawn.</param>
void DespawnUnit(int unitId);
        /// <summary>
/// Retrieves the unit with the specified identifier as the requested unit type.
/// </summary>
/// <param name="unitId">The unique identifier of the unit to retrieve.</param>
/// <returns>The unit with the specified identifier cast to <typeparamref name="T"/>.</returns>
T GetUnit<T>(int unitId) where T : BaseUnit;
        /// <summary>
/// Update the unit manager's state for the current frame.
/// </summary>
/// <param name="elapsedTime">Time in seconds since the previous update (delta time).</param>
/// <param name="now">Current game time in seconds.</param>
void OnGameUpdate(float elapsedTime, float now);
        /// <summary>
/// Initiates or updates movement for the specified unit, moving it from startPosition to nextPosition.
/// </summary>
/// <param name="unitId">Identifier of the unit to move.</param>
/// <param name="startPosition">World position where the movement begins.</param>
/// <param name="nextPosition">World position where the unit should move to.</param>
/// <param name="time">Movement start timestamp in seconds, used for synchronizing or interpolating the movement.</param>
void Move(int unitId, Vector3 startPosition, Vector3 nextPosition, double time);
        /// <summary>
/// Stops movement for the specified unit, setting its final position and facing at the given time.
/// </summary>
/// <param name="unitId">Identifier of the unit whose movement should be stopped.</param>
/// <param name="position">Final world position where the unit will be placed.</param>
/// <param name="direction">World-space facing direction to apply after stopping.</param>
/// <param name="time">Synchronization timestamp for the stop event.</param>
void StopMove(int unitId, Vector3 position, Vector3 direction, double time);
        /// <summary>
/// Executes an action skill for the specified unit using the given action button, position, direction, and start time.
/// </summary>
/// <param name="unitId">Identifier of the unit performing the action.</param>
/// <param name="buttonType">The action/skill button indicating which skill to perform.</param>
/// <param name="position">World-space position relevant to the action (e.g., target point or origin).</param>
/// <param name="direction">Direction vector for the action's orientation or targeting.</param>
/// <param name="startTime">Timestamp in seconds representing when the action started.</param>
void ActionSkill(int unitId, ActionButtonType buttonType, Vector3 position, Vector3 direction, double startTime);
    }
}