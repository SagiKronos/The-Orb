namespace TheOrb.Control
{
    public interface IRayCastable
    {
        bool HandleRaycast(PlayerController player);

        CursorType GetCursorType();
    }
}