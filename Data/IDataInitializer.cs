// DataManager�� ���� �޾ƿ� �ܺ� ����ȭ ���� Ŭ������ ���ο��� ����ϴ� Ŭ������ �ٲٴ� ������ �ϴ� �������̽�.
public interface IDataInitializer<T> where T : DataTypeBase
{
    public void Initialize(T data);
}