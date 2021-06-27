using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private int damage;

    void OnCollisionEnter2D(Collision2D collision) {
        Unit targetUnit = collision.gameObject.GetComponent<Unit>();
        if(targetUnit != null) {
            targetUnit.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    public void FireAt(Vector3 destination, int damage) {
        this.damage = damage;

        Vector3Int destinationInt = Vector3Int.FloorToInt(destination);
        Vector3 direction = (destinationInt - transform.position).normalized;
        Vector3Int directionInt = Vector3Int.RoundToInt(direction);

        StartCoroutine(Move(directionInt));
    }

    private IEnumerator Move(Vector3Int direction) {
        for(;;) {
            Vector3Int currentPosition = Vector3Int.FloorToInt(
                transform.position
            );
            Vector3Int newPosition = currentPosition + direction;

            transform.position = newPosition;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
