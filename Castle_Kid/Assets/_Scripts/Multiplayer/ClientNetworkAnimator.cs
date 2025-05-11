using Unity.Netcode.Components;

namespace _Scripts.Multiplayer
{
    public class ClientNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
