/*
        외부데이터 직렬화를 위한 클래스입니다.

        지금 보면 FolderName 을 밖에서 숫자로 받고 있는데,

        이 부분이 문자로 받으면 오타 문제가 심할거라고 생각해서 이렇게 바꿨지만

        조금 더 좋은 방법이 있지 않았을까 싶습니다

        다음에는 좀 더 좋은 방법으로 짜 보려고 합니다.
        
        이 부분은 전부 제가 작성했습니다.
*/


using Newtonsoft.Json;
using System.Collections.Generic;

[System.Serializable]
public abstract class DataTypeBase
    // 모든 외부 직렬화 데이터의 기반이 되는 클래스. 최소한의 요소만 들어 있다.
{
    private enum FolderName
    {
        Dummy = 0,
        MonsterSprite = 20,
        MonsterAnimator = 21,

        NPCSprite = 31,

        CardSprite = 41,
        ItemSprite = 42,
        ActionSprite = 60,
        ActionAnimator = 61,
        BuffSprite = 70,
        UI = 81,
        ObjectPool = 82,
        BGM = 90,
        SFX = 91,
        Room = 100
    }

    public int key;
    public string name;
    public string description;

    [JsonIgnore] private List<FolderName> folderName;

    [JsonProperty] private List<int> folderIndex;
    [JsonProperty] private List<string> filename;

    [JsonConstructor]
    public DataTypeBase(int key, string name, string description, List<int> folderIndex, List<string> filename)
    {
        this.key = key;
        this.name = name;
        this.description = description;
        this.folderIndex = folderIndex;
        this.filename = filename;

        folderName = new();
        for (int i = 0; i < folderIndex.Count; i++)
        {
            if ((FolderName)folderIndex[i] == FolderName.Dummy) // 더비 주소 제외.
                continue;

            folderName.Add((FolderName)folderIndex[i]);
        }

        this.filename.RemoveAll(x => x.Equals("null")); // 더미 파일명 제거.
    }


    public string GetPath(int index) { return $"{folderName[index]}/{filename[index]}"; }
}
