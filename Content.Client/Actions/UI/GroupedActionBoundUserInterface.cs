using Content.Client.Stylesheets.Palette;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Actions;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Prototypes;
using Content.Shared.Changeling.Components;
using Content.Shared.Changeling.Systems;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Actions.UI;

[UsedImplicitly]
public sealed partial class GroupedActionBoundUserInterface : BoundUserInterface
{
    [Dependency] private IUserInterfaceManager _userInterfaceManager = default!;

    private SimpleRadialMenu? _menu;
    private static readonly Color OffCooldownOptionBackground = Palettes.Green.Element.WithAlpha(128);
    private static readonly Color OnCooldownOptionBackground = Palettes.Slate.Element.WithAlpha(128);
    private static readonly Color OffCooldownHoverBackground = Palettes.Green.HoveredElement.WithAlpha(128);
    private static readonly Color OnCooldownHoverBackground = Palettes.Slate.HoveredElement.WithAlpha(128);
    private readonly ActionsSystem _actions;
    private readonly ActionUIController _actionUi;

    public GroupedActionBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _actions =  EntMan.System<ActionsSystem>();
        _actionUi = _userInterfaceManager.GetUIController<ActionUIController>();
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<SimpleRadialMenu>();
        Update();
        _menu.OpenOverMouseScreenPosition();
    }

    public override void Update()
    {
        if (_menu == null)
            return;

        if (!EntMan.TryGetComponent<GroupingActionComponent>(Owner, out var groupedAction))
            return;

        if (!EntMan.TryGetComponent<ActionComponent>(Owner, out var actionComp))
            return;

        if (!EntMan.TryGetComponent<ActionsComponent>(actionComp.AttachedEntity, out var actions))
            return;

        var models = ConvertToButtons(actions.Actions, groupedAction.Group);

        _menu.SetButtons(models);
    }

    private IEnumerable<RadialMenuOptionBase> ConvertToButtons(
        IEnumerable<EntityUid> actions,
        ProtoId<ActionGroupPrototype> group)
    {
        var buttons = new List<RadialMenuOptionBase>();

        foreach (var action in actions)
        {
            if (!EntMan.TryGetComponent<GroupedActionComponent>(action, out var grouped))
                continue;

            if (grouped.Group != group)
                continue;

            if (!EntMan.TryGetComponent<ActionComponent>(action, out var actionComp))
                continue;

            var onCooldown = _actions.IsCooldownActive(actionComp);

            // Options for selecting identities.
            var option = new RadialMenuActionOption<Entity<ActionComponent>>(SendActionRequest, (action, actionComp))
            {
                IconSpecifier = RadialMenuIconSpecifier.With(action),
                ToolTip = Loc.GetString("changeling-transform-bui-select-entity", ("entity", action)),
                BackgroundColor = onCooldown ? OnCooldownOptionBackground : OffCooldownOptionBackground, // mark as selected
                HoverBackgroundColor = onCooldown ? OnCooldownHoverBackground : OffCooldownHoverBackground,
            };
            buttons.Add(option);
        }

        return buttons;
    }

    private void SendActionRequest(Entity<ActionComponent> action)
    {
        _actionUi.TriggerAction(action);
    }
}
