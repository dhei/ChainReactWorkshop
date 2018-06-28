using ReactNative.Bridge;
using ReactNative.Modules.Core;
using ReactNative.UIManager;
using System.Collections.Generic;

namespace ChainReactWorkshop
{
    class RNOnnxPackage : IReactPackage
    {
        public IReadOnlyList<INativeModule> CreateNativeModules(ReactContext reactContext)
        {
            return new List<INativeModule>
            {
                new RNOnnxModule(),
            };
        }

        public IReadOnlyList<IViewManager> CreateViewManagers(ReactContext reactContext)
        {
            return new List<IViewManager>(0);
        }
    }
}
