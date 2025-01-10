using Newtonsoft.Json;
using System.Collections.Generic;

[System.Serializable]
public abstract class DataTypeBase
    // ��� �ܺ� ����ȭ �������� ����� �Ǵ� Ŭ����. �ּ����� ��Ҹ� ��� �ִ�.
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
            if ((FolderName)folderIndex[i] == FolderName.Dummy) // ���� �ּ� ����.
                continue;

            folderName.Add((FolderName)folderIndex[i]);
        }

        this.filename.RemoveAll(x => x.Equals("null")); // ���� ���ϸ� ����.
    }


    public string GetPath(int index) { return $"{folderName[index]}/{filename[index]}"; }
}