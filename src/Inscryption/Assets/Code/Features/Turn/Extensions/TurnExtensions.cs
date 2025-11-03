using Entitas;

namespace Code.Features.Turn.Extensions
{
  public static class TurnExtensions
  {
    public static (GameEntity attacker, GameEntity defender) GetBattleParticipants(
      this GameContext game,
      IGroup<GameEntity> heroes,
      IGroup<GameEntity> enemies)
    {
      GameEntity hero = heroes.GetSingleEntity();
      GameEntity enemy = enemies.GetSingleEntity();

      if (enemy?.isEnemyTurn == true)
        return (enemy, hero);

      return (hero, enemy);
    }

    public static bool IsActiveAttacker(this GameEntity entity)
    {
      if (entity == null)
        return false;

      return entity.isHero ? entity.isHeroTurn : entity.isEnemyTurn;
    }
  }
}
