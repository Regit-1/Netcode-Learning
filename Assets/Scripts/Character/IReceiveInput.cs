using UnityEngine;

public interface IReceiveInput
{
    void NetworkTick(CharacterInput input);
    void NetworkPredictionTick(CharacterInput input);
}
