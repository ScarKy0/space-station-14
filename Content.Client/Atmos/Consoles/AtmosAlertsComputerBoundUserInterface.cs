using Content.Shared.Atmos.Components;

namespace Content.Client.Atmos.Consoles;

public sealed class AtmosAlertsComputerBoundUserInterface : BoundUserInterface
{

    public AtmosAlertsComputerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey) { }

    protected override void Open()
    {
        base.Open();


    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        var castState = (AtmosAlertsComputerBoundInterfaceState) state;

        EntMan.TryGetComponent<TransformComponent>(Owner, out var xform);
    }

    public void SendFocusChangeMessage(NetEntity? netEntity)
    {
        SendMessage(new AtmosAlertsComputerFocusChangeMessage(netEntity));
    }

    public void SendDeviceSilencedMessage(NetEntity netEntity, bool silenceDevice)
    {
        SendMessage(new AtmosAlertsComputerDeviceSilencedMessage(netEntity, silenceDevice));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;

    }
}
