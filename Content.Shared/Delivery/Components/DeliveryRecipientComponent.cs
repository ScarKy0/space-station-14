using Robust.Shared.GameStates;

namespace Content.Shared.Delivery;

/// <summary>
/// Component given to deliveries.
/// Indicates the recipient of the delivery, used for the label, examine text and sprite layers.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DeliveryRecipientComponent : Component
{
    /// <summary>
    /// The name of the recipient of this delivery.
    /// Used for the examine text.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? RecipientName;

    /// <summary>
    /// The job of the recipient of this delivery.
    /// Used for the examine text.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? RecipientJobTitle;

    /// <summary>
    /// The EntityUid of the station this delivery was spawned on.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? RecipientStation;

    /// <summary>
    /// Whether the delivery should have a label of the recipient's name applied.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool LabelAfterRecipient = true;

    /// <summary>
    /// Whether the delivery should get recipient job icon applied.
    /// Requires the sprite component to have the respective layer.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool ApplySpriteLayers = true;
}
