using contract.Types;

namespace eventbuffer_contract.Events;

public interface IHasGameMetadata
{
    public GameMetadata Metadata {get; set;}
}