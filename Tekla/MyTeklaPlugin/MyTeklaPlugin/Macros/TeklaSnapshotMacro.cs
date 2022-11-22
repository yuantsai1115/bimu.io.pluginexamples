namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            akit.Callback("diaDisplaySnapshotDialog", "", "main_frame");
            akit.CommandEnd();
            akit.CommandStart("ail_snapshot", "", "main_frame");
            akit.PushButton("Pushbutton", "snapshot_dialog");
            akit.ValueChange("snapshot_dialog", "snapshot_selection", "2");
            akit.PushButton("options", "snapshot_dialog");
            akit.ValueChange("snapshot_option_dialog", "width", "500.0");
            akit.ValueChange("snapshot_option_dialog", "dpi", "72");
            akit.PushButton("option_apply", "snapshot_option_dialog");
            akit.PushButton("option_ok", "snapshot_option_dialog");
            akit.ValueChange("snapshot_dialog", "target_selection", "0");
            akit.PushButton("take_snapshot", "snapshot_dialog");
            akit.PushButton("cancel", "snapshot_dialog");
        }
    }
}
