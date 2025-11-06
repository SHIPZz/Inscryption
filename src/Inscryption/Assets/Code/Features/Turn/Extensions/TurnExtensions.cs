using Entitas;

namespace Code.Features.Turn.Extensions
{
  public static class TurnExtensions
  {
    public static (GameEntity attacker, GameEntity defender) GetBattleParticipants(
      IGroup<GameEntity> heroes,
      IGroup<GameEntity> enemies)
    {
      GameEntity hero = null;
      GameEntity enemy = null;

      foreach (GameEntity h in heroes)
      {
        hero = h;
        break;
      }

      foreach (GameEntity e in enemies)
      {
        enemy = e;
        break;
      }

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

    public static GameEntity GetCurrentPlayer(
      IGroup<GameEntity> heroes,
      IGroup<GameEntity> enemies)
    {
      foreach (GameEntity hero in heroes)
      {
        if (hero.isHeroTurn)
          return hero;
      }

      foreach (GameEntity enemy in enemies)
      {
        if (enemy.isEnemyTurn)
          return enemy;
      }

      return null;
    }
  }
}
