using Game.UI;
using System;

public interface ICursorManager
{
    public event Action<CursorState> CursorChanged;
}
