using UnityEngine;
// 예시 클래스
public class ExampleMonster : MonoBehaviour, IDataInitializer<ExampleMonsterData>
{
    public void Initialize(ExampleMonsterData data)
    {
        // ExampleMonsterData 안의 변수에 따라 ExampleMonster를 초기화
    }
}
