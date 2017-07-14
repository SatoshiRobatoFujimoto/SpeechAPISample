using HUX.Interaction;
using HUX.Receivers;
using UnityEngine;

public class TapMessageReceiverJa : InteractionReceiver
{
    protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
    {
        SingletonText.Instance.ClickButton_Ja();
    }
}
