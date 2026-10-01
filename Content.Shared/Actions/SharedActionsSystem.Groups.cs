using System.Diagnostics.CodeAnalysis;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Actions;

public abstract partial class SharedActionsSystem
{
    [Dependency] private SharedUserInterfaceSystem _ui = default!;

    private const string GroupedActionXmlGeneratedName = "GroupedActionBoundUserInterface";

    [SubscribeLocalEvent]
    private void OnGroupInit(Entity<GroupingActionComponent> ent, ref MapInitEvent args)
    {
        var userInterfaceComp = EnsureComp<UserInterfaceComponent>(ent);
        _ui.SetUi((ent, userInterfaceComp), GroupedActionUiKey.Key, new InterfaceData(GroupedActionXmlGeneratedName));
    }

    [SubscribeLocalEvent]
    private void OnGroupedActionAdded(Entity<GroupedActionComponent> ent, ref ActionGotAttachedEvent args)
    {
        if (!ProtoMan.TryIndex(ent.Comp.Group, out var proto))
            return;

        if (TryGetGroupingAction(args.Owner, ent.Comp.Group, out _))
            return;

        AddAction(args.Owner, proto.GroupAction);
    }

    [SubscribeLocalEvent]
    private void OnGroupedActionRemoved(Entity<GroupedActionComponent> ent, ref ActionGotDetachedEvent args)
    {
        // Don't remove if there are other actions in the group.
        if (GetActionsInGroup(args.Owner, ent.Comp.Group).Count > 0)
            return;

        // Can't remove a grouping action if it doesn't exist.
        if (!TryGetGroupingAction(args.Owner, ent.Comp.Group, out var grouping))
            return;

        RemoveAction(args.Owner, grouping);
    }

    [SubscribeLocalEvent]
    private void OnGroupingAction(Entity<GroupingActionComponent> ent, ref GroupedActionOpenRadialEvent args)
    {
        if (!TryComp<UserInterfaceComponent>(ent, out var userInterfaceComp))
            return;

        if (!_ui.IsUiOpen((ent, userInterfaceComp), GroupedActionUiKey.Key, args.Performer))
        {
            _ui.OpenUi((ent, userInterfaceComp), GroupedActionUiKey.Key, args.Performer);
        }
    }

    public bool TryGetActionsInGroup(Entity<ActionsComponent?> entity, ProtoId<ActionGroupPrototype> protoId, out HashSet<EntityUid> grouping)
    {
        grouping = new HashSet<EntityUid>();
        if (!Resolve(entity, ref entity.Comp, false))
            return false;

        foreach (var action in entity.Comp.Actions)
        {
            if (!TryComp<GroupedActionComponent>(action, out var groupedComp))
                continue;

            if (groupedComp.Group == protoId)
            {
                grouping.Add(action);
            }
        }

        return grouping.Count > 0;
    }

    public HashSet<EntityUid> GetActionsInGroup(Entity<ActionsComponent?> entity, ProtoId<ActionGroupPrototype> protoId)
    {
        if (!TryGetActionsInGroup(entity, protoId, out var grouping))
            return new HashSet<EntityUid>();

        return grouping;
    }

    public bool TryGetGroupingAction(Entity<ActionsComponent?> entity, ProtoId<ActionGroupPrototype> protoId, [NotNullWhen(true)] out EntityUid? grouping)
    {
        grouping = null;
        if (!Resolve(entity, ref entity.Comp, false))
            return false;

        var found = false;

        foreach (var action in entity.Comp.Actions)
        {
            if (!TryComp<GroupingActionComponent>(action, out var groupedComp))
                continue;

            if (groupedComp.Group == protoId)
            {
                found = true;
                grouping = action;
                break;
            }
        }

        return found;
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
