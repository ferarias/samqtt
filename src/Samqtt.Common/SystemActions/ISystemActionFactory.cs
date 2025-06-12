using System.Collections.Generic;

namespace Samqtt.SystemActions
{
    public interface ISystemActionFactory
    {
        IEnumerable<ISystemAction> GetEnabledActions();
    }

}
