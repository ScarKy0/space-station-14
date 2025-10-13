using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Delivery;

/// <summary>
/// Component given to deliveries.
/// Means the entity is a delivery, which upon opening will grant a reward to cargo.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DeliveryCargoRewardComponent : Component
{
    /// <summary>
    /// The base amount of spesos that gets added to the station bank account on unlock.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int BaseSpesoReward = 500;

    /// <summary>
    /// The bank account ID to grant the reward to.
    /// If null, will be split according to the funding allocations.
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<CargoAccountPrototype>? RewardBankAccount;

    /// <summary>
    /// Whether this reward was already granted.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool WasAwareded;
}
