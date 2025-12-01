using UnityEngine;

public class ArcaneBoltProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 5;

    private Transform target;
    private float fixedY;

    public float hitDistance = 1.0f;

    public void Initialize(Transform targetTransform, int dmg)
    {
        target = targetTransform;
        damage = dmg;
        fixedY = transform.position.y;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = target.position;
        targetPos.y = fixedY;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        float dist = Vector3.Distance(
            new Vector3(transform.position.x, fixedY, transform.position.z),
            targetPos
        );

        if (dist < hitDistance)
        {
            ApplyDamage();
            Destroy(gameObject);
        }
    }

    private void ApplyDamage()
    {
        EnemyController enemy = target.GetComponent<EnemyController>();
        if (enemy != null)
            enemy.TakeDamage(damage);
    }
}
