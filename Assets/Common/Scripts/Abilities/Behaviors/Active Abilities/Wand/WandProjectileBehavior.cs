using UnityEngine;
using OctoberStudio;
using System.Collections.Generic;

public class WandProjectileBehavior : SimplePlayerProjectileBehavior
{
    private int remainingBounces;
    private float bounceRadius;
    private List<GameObject> alreadyHit = new List<GameObject>();

    public void InitBounce(Vector2 position, Vector2 direction, float speed, float lifeTime, float damage, int? bounceCount, float radius)
    {
        transform.position = position;
        transform.localScale = Vector3.one * PlayerBehavior.Player.SizeMultiplier;

        this.direction = direction;
        this.Speed = speed;
        this.LifeTime = lifeTime;

        if (bounceCount.HasValue)
            this.remainingBounces = bounceCount.Value;

        this.DamageMultiplier = damage; // This is now treated as the final flat damage value
        this.bounceRadius = radius;
        this.spawnTime = Time.time;
        selfDestructOnHit = false;
        alreadyHit.Clear();

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
        if (!collision.CompareTag("Enemy")) return;
        if (alreadyHit.Contains(collision.gameObject)) return;

        alreadyHit.Add(collision.gameObject);

        if (collision.TryGetComponent(out EnemyBehavior enemy))
        {
            float actualDamage = DamageMultiplier; // ✅ Scriptable flat damage
            enemy.TakeDamage(actualDamage);
            Debug.Log($"[Wand] Hit {enemy.name} for {actualDamage}");
        }

        if (remainingBounces > 0)
        {
            remainingBounces--;

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

            if (nextEnemy != null)
            {
                Vector2 bounceDirection = (nextEnemy.Center - (Vector2)transform.position).normalized;
                InitBounce(transform.position, bounceDirection, Speed, LifeTime, DamageMultiplier, null, bounceRadius);
                return;
            }
        }

        FinishProjectile();
    }

    private void FinishProjectile()
    {
        Clear();
        onFinished?.Invoke(this);
    }
}
