using System.Linq;
using System.Numerics;
using Content.Shared.Actions;
using Content.Shared.Changeling.Components;
using Content.Shared.Cuffs;
using Content.Shared.Ensnaring;
using Content.Shared.Fluids;
using Content.Shared.IdentityManagement;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;

namespace Content.Shared.Changeling.Systems;

public sealed partial class ChangelingAbilitySystem : EntitySystem
{
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedCuffableSystem _cuffable = default!;
    [Dependency] private SharedEnsnareableSystem _snare = default!;
    [Dependency] private PullingSystem _pulling = default!;
    [Dependency] private SharedStunSystem _stun = default!;
    [Dependency] private SharedPuddleSystem _puddle = default!;
    [Dependency] private SharedChangelingIdentitySystem _changelingIdentity = default!;
    [Dependency] private ChangelingDevourSystem _changelingDevour = default!;
    [Dependency] private SharedTransformSystem _xform = default!;
    [Dependency] private SharedGunSystem _gun = default!;
    [Dependency] private INetManager _net = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChangelingBiodegradeAbilityComponent, ChangelingBiodegradeActionEvent>(OnBiodegradeAction);
        SubscribeLocalEvent<ChangelingIdentityComponent, ChangelingStingDnaEvent>(OnStingDna);
    }

    #region Biodegrade
    private void OnBiodegradeAction(Entity<ChangelingBiodegradeAbilityComponent> ent, ref ChangelingBiodegradeActionEvent args)
    {
        // We used biodegrade on self, so we wanna drop our cuffs and restraints
        if (args.Entity == args.Performer)
        {
            BiodegradeFreedom(ent, ref args);
            return;
        }

        // If not used on self, spit acid.
        BiodegradeProjectile(ent, ref args);
    }

    /// <summary>
    /// Handles the self-interaction of biodegrade. Namely freeing from cuffs and spawning a puddle.
    /// We pass in the arguments as reference so we can handle the event from within.
    /// </summary>
    private void BiodegradeFreedom(Entity<ChangelingBiodegradeAbilityComponent> ent, ref ChangelingBiodegradeActionEvent args)
    {
        // Nothing can be done :(
        if (!_cuffable.IsCuffed(args.Performer) && !_snare.IsEnsnared(args.Performer))
            return;

        if (_pulling.GetPuller(args.Performer) is { } puller)
        {
            _stun.TryAddParalyzeDuration(puller, ent.Comp.PullerStunDuration);
        }

        var toDelete = new List<EntityUid>();

        _cuffable.TryGetAllCuffs(args.Performer, out var cuffs);
        foreach (var cuff in cuffs.ToList())
        {
            _cuffable.Uncuff(args.Performer, args.Performer, cuff);
            toDelete.Add(cuff);
        }

        toDelete.AddRange(_snare.ForceFreeAll(args.Performer));

        args.Handled = true;

        var selfPopup = Loc.TryGetString(ent.Comp.ActivatedPopupSelf, out var self, ("user", Identity.Entity(args.Performer, EntityManager)), ("restraint", toDelete.First())) ? self : null;
        var othersPopup = Loc.TryGetString(ent.Comp.ActivatedPopup, out var others, ("user", Identity.Entity(args.Performer, EntityManager)), ("restraint", toDelete.First())) ? others : null;

        _popup.PopupPredicted(selfPopup, othersPopup, args.Performer, args.Performer, PopupType.LargeCaution);
        _audio.PlayPredicted(ent.Comp.SelfSound, args.Performer, args.Performer);

        foreach (var deleted in toDelete)
        {
            PredictedQueueDel(deleted);
        }

        if (ent.Comp.SpillSolution != null)
            _puddle.TrySpillAt(args.Performer, ent.Comp.SpillSolution, out _, false);
    }

    private void BiodegradeProjectile(Entity<ChangelingBiodegradeAbilityComponent> ent, ref ChangelingBiodegradeActionEvent args)
    {
        var mapPos = _xform.ToMapCoordinates(Transform(args.Performer).Coordinates);

        // Shooting isnt predicted :(
        if (_net.IsServer)
        {
            var entity = Spawn(ent.Comp.Projectile, mapPos);
            var direction = _xform.ToMapCoordinates(args.Target).Position - mapPos.Position;

            _audio.PlayPvs(ent.Comp.ProjectileSound, args.Performer);
            _gun.ShootProjectile(entity, direction, Vector2.Zero, null, args.Performer, ent.Comp.ProjectileSpeed);
        }

        args.Cooldown = ent.Comp.ProjectileCooldown;
        args.Handled = true;
    }

    #endregion

    private void OnStingDna(Entity<ChangelingIdentityComponent> ent, ref ChangelingStingDnaEvent args)
    {
        if (args.Target == ent.Owner)
            return; // Can't sting yourself.

        if (!_changelingDevour.CanDevour(ent.Owner, args.Target, checkDead: false, checkProtected: false))
            return;

        _changelingIdentity.GrantIdentity(ent, args.Target);

        args.Handled = true;
    }
}

/// <summary>
/// Action event for the Dna sting ability. Used to grand the changeling an identity without devouring somebody.
/// </summary>
public sealed partial class ChangelingStingDnaEvent : EntityTargetActionEvent;
