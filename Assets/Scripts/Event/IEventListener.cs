namespace poorlord
{
    /// <summary>
    /// 이벤트 리스너
    /// </summary>
    public interface IEventListener
    {
        bool OnEvent(IEvent e);
    }
}