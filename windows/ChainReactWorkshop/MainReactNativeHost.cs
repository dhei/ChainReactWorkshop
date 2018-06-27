using ReactNative;
using ReactNative.Modules.Core;
using RNCamera;
using ReactNative.Shell;
using System.Collections.Generic;

namespace ChainReactWorkshop
{
    class MainReactNativeHost : ReactNativeHost
    {
        public override string MainComponentName => "ChainReactWorkshop";

#if !BUNDLE || DEBUG
        public override bool UseDeveloperSupport => true;
#else
        public override bool UseDeveloperSupport => false;
#endif

        protected override string JavaScriptMainModuleName => "index";

#if BUNDLE
        protected override string JavaScriptBundleFile => "ms-appx:///ReactAssets/index.windows.bundle";
#endif

        protected override List<IReactPackage> Packages => new List<IReactPackage>
        {
            new MainReactPackage(),
            new RNCameraPackage(),
        };
    }
}
