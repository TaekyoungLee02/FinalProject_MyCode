using System.Collections.Generic;

[System.Serializable]
public class ExampleMonsterData : DataTypeBase
{
    public int health;
    public int attack;
    public int defense;
    public ExampleMonsterData(int key, string name, string description, List<int> folderIndex, List<string> filename) : base(key, name, description, folderIndex, filename) { }
}