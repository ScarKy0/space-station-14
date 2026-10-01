using Content.Shared.Actions.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Actions.Components;

/// <summary>
/// Indicates that this action is part of a group and will be handled by another action's radial.
/// Still allows independent use, but is not added to the action UI by default.
/// </summary>
[NetworkedComponent, RegisterComponent, Access(typeof(SharedActionsSystem))]
public sealed partial class GroupedActionComponent : Component
{
    /// <summary>
    /// What action group this action belongs to.
    /// Used for grouping the several "sub-actions" inside one "bigger" action's radial menu.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<ActionGroupPrototype> Group;
}
