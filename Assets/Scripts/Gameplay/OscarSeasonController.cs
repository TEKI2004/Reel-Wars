using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class OscarSeasonController : MonoBehaviour
{
    private enum OscarSeasonState
    {
        Idle,
        Intro,
        Active,
        Outro
    }

    [Title("References")]
    [Required][SerializeField] private BaseBehaviour playerBase;
    [Required][SerializeField] private OscarSeasonView oscarView;
    [Required][SerializeField] private OscarSeasonTimerView timerView;
    [Required][SerializeField] private PopularityMetersController popularityMeters;

    [ShowInInspector, ReadOnly]
    private OscarSeasonState state = OscarSeasonState.Idle;

    private Popularity Popularity => playerBase.Player.Popularity;
    private PopularityConfig PopularityConfig => GameManager.Instance.Config.Popularity;
    private OscarSeasonConfig OscarConfig => GameManager.Instance.Config.OscarSeason;

    public float MoveSpeedMultiplier { get; private set; } = 1f;
    public float AttackSpeedMultiplier { get; private set; } = 1f;

    private void EnableBuff()
    {
        MoveSpeedMultiplier = GameManager.Instance.Config.OscarSeason.MoveSpeedMultiplier;
        AttackSpeedMultiplier = GameManager.Instance.Config.OscarSeason.AttackSpeedMultiplier;
    }

    private void DisableBuff()
    {
        MoveSpeedMultiplier = 1f;
        AttackSpeedMultiplier = 1f;
    }

    private void OnEnable()
    {
        Popularity.OnPointsChanged += HandlePopularityChanged;
    }

    private void OnDisable()
    {
        Popularity.OnPointsChanged -= HandlePopularityChanged;
    }

    private void HandlePopularityChanged(int points)
    {
        if (state != OscarSeasonState.Idle) return;
        if (points < PopularityConfig.MaxPoints) return;

        StartCoroutine(OscarSeasonRoutine());
    }

    private IEnumerator OscarSeasonRoutine()
    {
        state = OscarSeasonState.Intro;
        Popularity.SetGainEnabled(false);
        popularityMeters.SetOscarStatus(playerBase.OwnerId, true);

        yield return oscarView.PlayIntro();
        yield return timerView.PlayIntro();

        state = OscarSeasonState.Active;

        EnableBuff();

        yield return timerView.PlayTimer(OscarConfig.Duration);

        DisableBuff();

        state = OscarSeasonState.Outro;

        yield return timerView.PlayOutro();
        yield return oscarView.PlayOutro();

        Popularity.ResetAfterOscarSeason();
        Popularity.SetGainEnabled(true);
        popularityMeters.SetOscarStatus(playerBase.OwnerId, false);

        state = OscarSeasonState.Idle;
    }

    [Button(ButtonSizes.Large)]
    private void TestOscarSeason()
    {
        if (!Application.isPlaying || state != OscarSeasonState.Idle) return;
        StartCoroutine(OscarSeasonRoutine());
    }
}