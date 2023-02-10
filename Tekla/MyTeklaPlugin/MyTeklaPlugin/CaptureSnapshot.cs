#pragma warning disable 1633 // Unrecognized #pragma directive
#pragma reference "Tekla.Macros.Akit"
#pragma reference "Tekla.Macros.Wpf.Runtime"
#pragma reference "Tekla.Macros.Runtime"
#pragma warning restore 1633 // Unrecognized #pragma directive

namespace UserMacros {
    public sealed class Macro {
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime) {
            Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
            akit.Callback("acmdZoomToSelected", "", "View_01 window_1");
            wpf.InvokeCommand("CommandRepository", "Tools.Screenshot");
            //akit.PushButton("options", "snapshot_dialog");
            //akit.ValueChange("snapshot_option_dialog", "width", "520");
            //akit.ValueChange("snapshot_option_dialog", "height", "420.000000000000");
            //akit.PushButton("option_ok", "snapshot_option_dialog");
            //akit.FileSelection("snapshots\\beam1");
            //akit.PushButton("browse", "snapshot_dialog");
            akit.ValueChange("snapshot_dialog", "target_selection", "0");
            //akit.ValueChange("snapshot_dialog", "filename", "snapshots\\beam1.png");
            akit.PushButton("take_snapshot", "snapshot_dialog");
            //akit.PushButton("cancel", "snapshot_dialog");
        }
    }
}
