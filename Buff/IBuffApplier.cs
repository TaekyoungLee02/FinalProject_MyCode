/*
        버프를 캡슐화하는 인터페이스입니다.

        버프 안의 내용을 다른 객체에서 수정할 수 없게 하기 위해 만들었습니다.

        이 부분은 전부 제가 작성했습니다.
*/

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
