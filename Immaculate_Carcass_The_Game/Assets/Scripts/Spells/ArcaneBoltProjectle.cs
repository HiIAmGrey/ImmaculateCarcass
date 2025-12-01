using UnityEngine;

public class ArcaneBoltProjectile : MonoBehaviour
{
    public float speed = 10f;     // clean, controlled projectile speed
    public int damage = 5;        // set from PlayerCombat

    private Transform target;     // who the bolt is moving toward
    private float fixedY;         // locked Y height (so it never sinks/rises)

    // Called by PlayerCombat right after Instantiate()
    public void Initialize(Transform targetTransform, int dmg)
    {
        target = targetTransform;
        damage = dmg;

        // bolt stays at exact spawn height
        fixedY = transform.position.y;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // target position on the horizontal plane only
        Vector3 targetPos = target.position;
        targetPos.y = fixedY;

        // move horizontally toward the target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // check distance with Y locked out
        Vector3 flatSelf = new Vector3(transform.position.x, fixedY, transform.position.z);
        float dist = Vector3.Distance(flatSelf, targetPos);

        if (dist < 0.3f)   // this was the original hit threshold before changing it
        {
            ApplyDamage();
            Destroy(gameObject);
        }
    }

    private void ApplyDamage()
    {
        EnemyController enemy = target.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
