using System.Collections;

public interface IBuffApplier
{
    /// <summary>
    /// Call When Buff Starts Applying
    /// </summary>
    /// <param name="status">CharacterStatus of Buff Applying Object</param>
    public void OnBuffStart(CharacterStatus status);

    /// <summary>
    /// Call Buff Update Every BuffIntervalSeconds
    /// </summary>
    /// <param name="status">CharacterStatus of Buff Applying Object</param>
    public IEnumerator OnBuffUpdate(CharacterStatus status);

    /// <summary>
    /// Call When Buff Ends
    /// </summary>
    /// <param name="status">CharacterStatus of Buff Applying Object</param>
    public void OnBuffEnd(CharacterStatus status);
}