using eventbuffer_contract.Types;

namespace eventbuffer_contract;

public static class EventBufferContract<TEvent>
{
    public delegate Task Append(TEvent e);

    public delegate Task<TEvent> Read();
}
