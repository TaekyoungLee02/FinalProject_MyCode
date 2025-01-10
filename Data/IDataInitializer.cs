// DataManager를 통해 받아온 외부 직렬화 전용 클래스를 내부에서 사용하는 클래스로 바꾸는 역할을 하는 인터페이스.
public interface IDataInitializer<T> where T : DataTypeBase
{
    public void Initialize(T data);
}