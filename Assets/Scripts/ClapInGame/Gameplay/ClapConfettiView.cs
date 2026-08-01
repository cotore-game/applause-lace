using UnityEngine;

/// <summary>
/// 成立した拍手1回につき、UI用の紙吹雪を一定数放出します。
/// ParticleSystemの連続Emissionは使用しません。
/// </summary>
public sealed class ClapConfettiView : MonoBehaviour
{
    [SerializeField] private ParticleSystem confettiEffects;
    [SerializeField, Min(1)] private int particlesPerClap = 6;

    private void Awake()
    {
        DisableContinuousEmission();
    }

    /// <summary>既存の粒子を消去し、次のラウンド開始前の状態へ戻します。</summary>
    public void Prepare()
    {
        if (confettiEffects == null)
        {
            return;
        }

        DisableContinuousEmission();
        confettiEffects.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    /// <summary>1拍手分の紙吹雪を即座に放出します。</summary>
    public void PlayBurst()
    {
        if (confettiEffects == null)
        {
            return;
        }

        if (!confettiEffects.isPlaying)
        {
            confettiEffects.Play(true);
        }

        confettiEffects.Emit(particlesPerClap);
    }

    private void DisableContinuousEmission()
    {
        if (confettiEffects == null)
        {
            return;
        }

        ParticleSystem.EmissionModule emission = confettiEffects.emission;
        emission.rateOverTime = 0f;
        emission.rateOverDistance = 0f;
    }
}
