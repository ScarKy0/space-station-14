using Content.Shared.Anomaly;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Content.Shared.Teleportation.Systems;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.Timing;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAERandomPortalSystem : BaseXAESystem<XAERandomPortalComponent>
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedAnomalySystem _anomaly = default!;
    [Dependency] private readonly LinkedEntitySystem _link = default!;

    /// <inheritdoc />
    protected override void OnActivated(Entity<XAERandomPortalComponent> ent, ref XenoArtifactNodeActivatedEvent args)
    {
        if (!_timing.IsFirstTimePredicted)
            return;

        var random = SharedRandomExtensions.PredictedRandom(_timing, GetNetEntity(ent));

        var randomOrder = random.Next(1) == 1;

        var firstPortalProto = randomOrder ? ent.Comp.FirstPortalPrototype : ent.Comp.SecondPortalPrototype;
        var secondPortalProto = randomOrder ? ent.Comp.SecondPortalPrototype : ent.Comp.FirstPortalPrototype;

        if (Transform(ent).GridUid is { } grid)
        {
            var firstPortal = SpawnAtPosition(firstPortalProto, Transform(ent).Coordinates);
            var secondPortal = _anomaly.SpawnOnRandomGridLocation(grid, secondPortalProto);

            if (secondPortal == null)
            {
                QueueDel(firstPortal);
                return;
            }

            _link.TryLink(firstPortal, secondPortal.Value);

        }

        var xform = Transform(args.Artifact);
        _popup.PopupPredictedCoordinates(Loc.GetString("blink-artifact-popup"), xform.Coordinates, args.User, PopupType.Medium);
    }
}
