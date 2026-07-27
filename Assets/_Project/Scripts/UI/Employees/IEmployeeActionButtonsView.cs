using UnityEngine.UI;

public interface IEmployeeActionButtonsView
{
    public void SetActionButtonsInteractable(bool val);

    public Button GetMoveButton();
    public Button GetStopButton();
    public Button GetReturnButton();
}
