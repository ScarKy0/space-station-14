using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE.Components;

/// <summary>
/// When activated, will create portals linking the artifact and a random position.
/// </summary>
[RegisterComponent, Access(typeof(XAERandomPortalSystem)), NetworkedComponent]
public sealed partial class XAERandomPortalComponent : Component
{
    [DataField]
    public EntProtoId FirstPortalPrototype = "PortalBlackHole";

    [DataField]
    public EntProtoId SecondPortalPrototype = "PortalWhiteHole";
}
