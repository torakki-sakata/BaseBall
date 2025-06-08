using Fusion;

public struct GameInput : INetworkInput
{
    public NetworkBool isSwinging;
    public NetworkBool changeStance;
    public NetworkBool rotateLeft;
    public NetworkBool moveUp;
    public NetworkBool moveDown;
    public NetworkBool throwBall;        // Alpha1
    public NetworkBool throwSlow;        // Alpha2
    public NetworkBool throwSlider;      // Alpha3
    public NetworkBool throwShoot;       // Alpha4
    public NetworkBool throwInvisible;   // V
    public NetworkBool throwStop;        // S
    public NetworkBool resetBall;        // O
}