using Content.Shared.Actions;
using Content.Shared.Chemistry.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Changeling.Components;

/// <summary>
/// Allows the changeling to vomit acid over restraints, setting themselves free and stunning whoever is pulling them.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ChangelingBiodegradeAbilityComponent : Component
{
    /// <summary>
    /// How long to stun the puller of the changeling when this action is activated.
    /// </summary>
    [DataField]
    public TimeSpan PullerStunDuration = TimeSpan.FromSeconds(3f);

    /// <summary>
    /// The projectile to shoot if biodegrade isn't used on self.
    /// </summary>
    [DataField]
    public EntProtoId Projectile = "BulletAcidStrong";

    /// <summary>
    /// The speed at which to fire the projectile if biodegrade isn't used on self.
    /// </summary>
    [DataField]
    public float ProjectileSpeed = 15f;

    /// <summary>
    /// Cooldown to use on the action if the projectile is fired, rather than using the default action cooldown.
    /// </summary>
    [DataField]
    public TimeSpan? ProjectileCooldown = TimeSpan.FromSeconds(10f);

    /// <summary>
    /// The popup to display over the user when the action is used.
    /// Only visible to other players.
    /// </summary>
    [DataField]
    public LocId ActivatedPopup = "changeling-biodegrade-used-popup";

    /// <summary>
    /// The popup to display over the user when the action is used.
    /// Only visible to the user
    /// </summary>
    [DataField]
    public LocId ActivatedPopupSelf = "changeling-biodegrade-used-popup-self";

    /// <summary>
    /// The sound to play when the ability is successfully used on self.
    /// </summary>
    [DataField]
    public SoundSpecifier SelfSound = new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg");

    /// <summary>
    /// The sound to play when the projectile is shot.
    /// </summary>
    [DataField]
    public SoundSpecifier ProjectileSound = new SoundPathSpecifier("/Audio/Weapons/Xeno/alien_spitacid.ogg");

    /// <summary>
    /// The reagents to spill at the location of the entity when this action is used.
    /// </summary>
    [DataField]
    public Solution? SpillSolution = new([new("SulfuricAcid", 30)]);
}

/// <summary>
/// Action event for Biodegrade, raised on the ability when used.
/// </summary>
public sealed partial class ChangelingBiodegradeActionEvent : WorldTargetActionEvent;
