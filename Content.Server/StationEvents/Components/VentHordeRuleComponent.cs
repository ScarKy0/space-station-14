using Content.Server.StationEvents.Events;
using Content.Shared.EntityTable.EntitySelectors;

namespace Content.Server.StationEvents.Components;

[RegisterComponent, Access(typeof(VentHordeRule))]
public sealed partial class VentHordeRuleComponent : Component
{
    /// <summary>
    /// The table of possible mobs to spawn from the vent.
    /// </summary>
    [DataField(required: true)]
    public EntityTableSelector Table = default!;

    /// <summary>
    /// The vent that has been chosen to spawn the entities.
    /// Spawning logic is handled by <see cref="VentHordeSpawnerComponent"/>
    /// </summary>
    [DataField]
    public EntityUid? ChosenVent;
}
