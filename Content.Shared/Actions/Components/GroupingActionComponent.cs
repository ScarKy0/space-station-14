using Content.Shared.Actions.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Actions.Components;

/// <summary>
/// Indicates this action can open a radial, containing other actions with <see cref="GroupedActionComponent"/> of the same <see cref="Group"/>.
/// </summary>
[NetworkedComponent, RegisterComponent, Access(typeof(SharedActionsSystem))]
public sealed partial class GroupingActionComponent : Component
{
    /// <summary>
    /// What action group this action manages.
    /// Used for grouping the several "sub-actions" inside one "bigger" action's radial menu.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<ActionGroupPrototype> Group;
}
