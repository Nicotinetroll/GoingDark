using UnityEngine;
using OctoberStudio;
using System.Collections.Generic;

public class WandProjectileBehavior : SimplePlayerProjectileBehavior
{
    private int remainingBounces;
    private float bounceRadius;
    private List<GameObject> alreadyHit = new List<GameObject>();
<<<<<<< Updated upstream
    private bool hasHit;
=======
>>>>>>> Stashed changes
    private const float damageFalloffPerBounce = 0.8f;

    public void InitBounce(Vector2 position, Vector2 direction, float speed, float lifeTime, float damage, int? bounceCount, float radius)
    {
        transform.position = position;
        transform.localScale = Vector3.one * PlayerBehavior.Player.SizeMultiplier;

        this.direction = direction;
        this.Speed = speed;
        this.LifeTime = lifeTime;

<<<<<<< Updated upstream
        // ✅ Only set bounce count if it's the FIRST init
=======
>>>>>>> Stashed changes
        if (bounceCount.HasValue)
            this.remainingBounces = bounceCount.Value;

        this.DamageMultiplier = damage;
        this.bounceRadius = radius;
        this.spawnTime = Time.time;
        this.hasHit = false;
        alreadyHit.Clear();
<<<<<<< Updated upstream
        selfDestructOnHit = false;
=======
>>>>>>> Stashed changes

        if (rotatingPart != null)
            rotatingPart.rotation = Quaternion.FromToRotation(Vector2.up, direction);

        if (trail != null)
            trail.Clear();

        foreach (var p in particles)
        {
            p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            p.Clear();
            p.Play();
        }

        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += direction * Time.deltaTime * Speed;

        if (LifeTime > 0 && Time.time - spawnTime > LifeTime)
        {
            FinishProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
<<<<<<< Updated upstream
        if (hasHit) return; // ✅ prevent double hit per enemy
=======
>>>>>>> Stashed changes
        if (!collision.CompareTag("Enemy")) return;
        if (alreadyHit.Contains(collision.gameObject)) return;

        hasHit = true;
        alreadyHit.Add(collision.gameObject);

        // ✅ Deal damage manually
        if (collision.TryGetComponent(out EnemyBehavior enemy))
        {
            float actualDamage = PlayerBehavior.Player.Damage * DamageMultiplier;
            enemy.TakeDamage(actualDamage);
            Debug.Log($"[Wand] Hit {enemy.name} for {actualDamage}");
        }

        // ✅ Handle bounce logic
        if (remainingBounces > 0)
        {
            EnemyBehavior nextEnemy = null;
            float closestDistance = float.MaxValue;

            var allEnemies = StageController.EnemiesSpawner.GetEnemiesInRadius(transform.position, bounceRadius);

            foreach (var candidate in allEnemies)
            {
                if (candidate == null || alreadyHit.Contains(candidate.gameObject)) continue;

                float dist = (candidate.Center - (Vector2)transform.position).sqrMagnitude;
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    nextEnemy = candidate;
                }
            }

<<<<<<< Updated upstream
=======
            EnemyBehavior nextEnemy = null;
            float closestDistance = float.MaxValue;

            var allEnemies = StageController.EnemiesSpawner.GetEnemiesInRadius(transform.position, bounceRadius);

            foreach (var candidate in allEnemies)
            {
                if (candidate == null || alreadyHit.Contains(candidate.gameObject)) continue;

                float dist = (candidate.Center - (Vector2)transform.position).sqrMagnitude;
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    nextEnemy = candidate;
                }
            }

>>>>>>> Stashed changes
            if (nextEnemy != null)
            {
                remainingBounces--;
                float nextDamage = DamageMultiplier * damageFalloffPerBounce;
                Vector2 bounceDirection = (nextEnemy.Center - (Vector2)transform.position).normalized;
<<<<<<< Updated upstream

                // ✅ Re-init for next bounce
                InitBounce(transform.position, bounceDirection, Speed, LifeTime, nextDamage, null, bounceRadius);
=======
                InitBounce(transform.position, bounceDirection, Speed, LifeTime, DamageMultiplier, null, bounceRadius);
>>>>>>> Stashed changes
                return;
            }
        }

        // ❌ No bounce or target? End the projectile
        FinishProjectile();
    }

    private void FinishProjectile()
    {
        Clear();
        onFinished?.Invoke(this);
    }
}
