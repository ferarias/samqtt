using System.Collections.Generic;

namespace Samqtt.SystemActions
{
    public interface ISystemActionFactory
    {
        IDictionary<string, ISystemAction> GetEnabledActions();
    }

}
