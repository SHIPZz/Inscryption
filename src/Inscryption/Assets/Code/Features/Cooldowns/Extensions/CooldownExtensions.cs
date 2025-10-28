namespace Code.Features.Cooldowns.Extensions
{
    public static class CooldownExtensions
    {
        public static GameEntity PutOnCooldown(this GameEntity cooldown, float value)
        {
            if (cooldown.hasCooldown)
                cooldown.ReplaceCooldown(value);
            else
                cooldown.AddCooldown(value);

            cooldown.ReplaceCooldownLeft(value);
            cooldown.isCooldownUp = false;
            
            return cooldown;
        }
    }
}