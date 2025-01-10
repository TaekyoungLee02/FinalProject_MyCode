/*
    이 StateMachine 과 BaseState 는 팀원이 작성한 부분이지만 제가 작성한 부분의 이해를 쉽게 하기 위해 같이 넣어 두었습니다.
*/

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
