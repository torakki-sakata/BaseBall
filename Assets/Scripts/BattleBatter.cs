using Fusion;
using UnityEngine;

public class BattleBatter : NetworkBehaviour
{
    [Networked] private bool IsRightStance { get; set; } = true;

    [SerializeField] private GameObject right1;
    [SerializeField] private GameObject left1;

    public override void Spawned()
    {
        UpdateStance();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if (GetInput(out BatterInput input))
        {
            if (input.Swing)
            {
                Swing();
            }

            if (input.ChangeStance)
            {
                IsRightStance = !IsRightStance;
                UpdateStance();
            }
        }
    }

    private void Swing()
    {
        Debug.Log("Swing!");

        // ここにバット振りアニメーション処理または力を加える処理を実装
        // 例：right1.GetComponent<Bat>().Swing();　など
    }

    private void UpdateStance()
    {
        if (right1 != null && left1 != null)
        {
            right1.SetActive(IsRightStance);
            left1.SetActive(!IsRightStance);
        }
    }
}
