using System;

public interface IUserInterfaceEventHandler<T>
{
    public Action<T> Started { get; set; }
    public Action<T> Performed { get; set; }
    public Action<T> Hold { get; set; }
    public Action<T> Canceled { get; set; }
}

public interface IUserInterfaceEventHandler
{
    public Action Started { get; set; }
    public Action Performed { get; set; }
    public Action Hold { get; set; }
    public Action Canceled { get; set; }
}