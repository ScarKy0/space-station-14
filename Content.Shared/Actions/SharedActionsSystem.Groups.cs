using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Changeling.Systems;
using Content.Shared.Ghost;
using Content.Shared.Ghost.Systems;
using Content.Shared.Mobs;
using Robust.Shared.Serialization;

namespace Content.Shared.Actions;

public abstract partial class SharedActionsSystem
{
    [Dependency] private SharedUserInterfaceSystem _ui = default!;

    private const string GroupedActionXmlGeneratedName = "GroupedActionBoundUserInterface";

    [SubscribeLocalEvent]
    private void OnGroupInit(Entity<GroupingActionComponent> ent, ref MapInitEvent args)
    {
        Log.Debug("Adding interface");
        var userInterfaceComp = EnsureComp<UserInterfaceComponent>(ent);
        _ui.SetUi((ent, userInterfaceComp), GroupedActionUiKey.Key, new InterfaceData(GroupedActionXmlGeneratedName));
    }

    [SubscribeLocalEvent]
    private void OnGroupingAction(Entity<GroupingActionComponent> ent, ref GroupedActionOpenRadialEvent args)
    {
        Log.Debug("Got event");
        if (!TryComp<UserInterfaceComponent>(ent, out var userInterfaceComp))
            return;

        if (!_ui.IsUiOpen((ent, userInterfaceComp), GroupedActionUiKey.Key, args.Performer))
        {
            Log.Debug("Opening UI");
            _ui.OpenUi((ent, userInterfaceComp), GroupedActionUiKey.Key, args.Performer);
        }
    }
}

[Serializable, NetSerializable]
public enum GroupedActionUiKey : byte
{
    Key,
}

/// <summary>
/// Action event for opening the grouped action radial menu.
/// </summary>
public sealed partial class GroupedActionOpenRadialEvent : InstantActionEvent;
