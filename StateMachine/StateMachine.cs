public class StateMachine
{
    public BaseState currentState;

    public void ChangeState(BaseState nextState)
    {
        currentState?.Exit();
        currentState = nextState;
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void FixedUpdate(float time)
    {
        currentState?.FixedUpdate(time);
    }
}
