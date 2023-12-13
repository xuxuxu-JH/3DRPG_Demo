//>观察者的共同行为 敌人监听玩家 收到死亡消息后 所做出的共同的行为
public interface IEndGameObserver
{
    void EndNotify();
}
