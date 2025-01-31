using UnityEngine;

public class Starling : Boss
{
    const float flipIntensitySoft = 0.01f;
    const float flipIntensityHard = 0.5f;
    const float targetingIntensitySoft = 0.05f;
    const float targetingIntensityHard = 0.2f;
    private Vector3 farTargetSight { get { return new Vector3(0, -1.2f, 1.9f); } }
    private Vector3 nearTargetSight { get { return new Vector3(0, -0.7f, 0.9f); } }
    private Vector3 upTargetSight { get { return new Vector3(0, -1.5f, 0); } }


    protected override void BossUpdate()
    {
        if (IsUniqueActing("softAim"))
        {
            TargetChange();
            TargetAimY(targetCursor, flipIntensitySoft);
        }
        if (IsUniqueActing("hardAim"))
        {
            TargetChange();
            TargetAimY(targetCursor, flipIntensityHard);
        }

        if (IsUniqueActing("farSight"))
        {
            TargetChange();
            TargetAimY(targetCursor, flipIntensityHard);
            GetLiveEntity().SetMovement((transform.InverseTransformPoint(targetCursor) - farTargetSight)
                * targetingIntensitySoft / Time.deltaTime);
        }
        if (IsUniqueActing("nearSight"))
        {
            TargetChange();
            TargetAimY(targetCursor, flipIntensityHard);
            GetLiveEntity().SetMovement((transform.InverseTransformPoint(targetCursor) - nearTargetSight)
                * targetingIntensityHard / Time.deltaTime);
        }
        if (IsUniqueActing("upSight"))
        {
            TargetChange();
            GetLiveEntity().SetMovement((transform.InverseTransformPoint(targetCursor) - upTargetSight)
                * targetingIntensitySoft / Time.deltaTime);
        }
    }
}
