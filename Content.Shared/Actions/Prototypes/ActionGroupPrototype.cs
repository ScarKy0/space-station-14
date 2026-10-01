using Robust.Shared.Prototypes;

namespace Content.Shared.Actions.Prototypes;

[Prototype]
public sealed partial class ActionGroupPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = null!;

    /// <summary>
    /// The action that will handle this group.
    /// Upon use, it should open a radial that displays all actions that belong to this group.
    /// Does not make the action dependent on the group, meaning it can still be used by itself.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId GroupAction;
}
