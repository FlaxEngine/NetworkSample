using FlaxEngine;
using FlaxEngine.GUI;

public class ExitOnEsc : Script
{
    public FontAsset Font;

    public override void OnEnable()
    {
#if !FLAX_EDITOR
        Style.Current.FontMedium = Font.CreateFont(10);
#endif
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyUp(KeyboardKeys.Escape))
            Engine.RequestExit();
    }
}
