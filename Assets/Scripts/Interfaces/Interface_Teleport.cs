using UnityEngine;
// ANYTHING USING THIS INTERFACE MUST HAVE A RIGID BODY ATTACHED!

public interface ITeleport
{
    void Teleport_To(Vector3 location, Vector3 direction);
}
