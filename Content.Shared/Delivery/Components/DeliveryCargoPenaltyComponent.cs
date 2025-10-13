using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Delivery;

/// <summary>
/// Component given to deliveries.
/// Means the entity is a delivery, which upon opening will grant a reward to cargo.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true)]
public sealed partial class DeliveryCargoPenaltyComponent : Component
{
    /// <summary>
    /// The base amount of spesos that will be removed from the station bank account on a penalized delivery
    /// </summary>
    [DataField, AutoNetworkedField]
    public int BaseSpesoPenalty = 250;

    /// <summary>
    /// The bank account ID of the account to subtract funds from in case of penalization
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<CargoAccountPrototype> PenaltyBankAccount = "Cargo";

    /// <summary>
    /// Whether this delivery has already received a penalty.
    /// Used to avoid getting penalized several times.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool WasPenalized;
}
