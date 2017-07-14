using HUX.Interaction;
using HUX.Receivers;
using UnityEngine;

public class TapMessageReceiverEn : InteractionReceiver
{
    protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
    {
        SingletonText.Instance.ClickButton_En();
    }
}
