using UnityEngine;

public interface IMoveTo 
{
    Transform transform { get; }
    void MoveTo(Vector3 position) => MoveAndRotateTo(position, transform.rotation);
    void RotateTo(Quaternion rotation) => MoveAndRotateTo(transform.position, rotation);
    void MoveAndRotateTo(Vector3 position, Quaternion rotation);
}